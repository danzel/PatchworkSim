import logging
import requests
import time
from tensorforce.agents import PPOAgent
from tensorforce.environments import Environment
from tensorforce.execution import Runner

obs_size = requests.get('http://localhost:5000/api/Patchwork/observation-size')
obs_size = int(obs_size.text)
print(f'obs_size is {obs_size}')

opponent_strength = 10

class PatchworkEnv(Environment):
    def __init__(self):
        self.session = requests.Session()
        self.wins = []

    def reset(self):
        req = self.session.post('http://localhost:5000/api/Patchwork/create', json = { "opponentStrength": opponent_strength })
        res = req.json()
        self.gameId = res['gameId']

        return res['observationForNextMove']

    def execute(self, action):
        move = int(action)
        req = self.session.post('http://localhost:5000/api/Patchwork/perform-move', json = { "gameId": self.gameId, "move": move })
        res = req.json()

        if res['gameHasEnded']:
            self.wins.append(1 - res['winningPlayer'])

        return (res['observationForNextMove'], res['gameHasEnded'], res['reward'])

    @property
    def states(self):
        return dict(shape=(obs_size,), type='float')

    @property
    def actions(self):
        return dict(type='int', num_actions=4)


logger = logging.getLogger(__name__)
logger.setLevel(logging.DEBUG)

console_handler = logging.StreamHandler()
console_handler.setFormatter(
    logging.Formatter("%(asctime)s [%(threadName)-12.12s] [%(levelname)-5.5s]  %(message)s"))
logger.addHandler(console_handler)

environment = PatchworkEnv()

# Create a Proximal Policy Optimization agent
agent = PPOAgent(
    states=environment.states,
    actions=environment.actions,
    network=[
        #dict(type='dense', size=64),
        dict(type='dense', size=64),
        dict(type='dense', size=64)
    ],
    update_mode=dict(
        unit= 'episodes',
        batch_size=20
    ),
    # PGModel
    baseline_mode='states',
    baseline=dict(
        type='mlp',
        sizes=[32, 32]
    ),
    baseline_optimizer=dict(
        type='multi_step',
        optimizer=dict(
            type='adam',
            learning_rate=1e-3
        ),
        num_steps=5
    ),
    gae_lambda=0.97,

    step_optimizer=dict(
        type='adam',
        learning_rate=1e-4
    )
)

runner = Runner(
    agent=agent,
    environment=environment
)

def episode_finished(r):
    if r.episode % 100 == 0:
        sps = r.timestep / (time.time() - r.start_time)
        logger.info("Finished episode {ep} after {ts} timesteps. Steps Per Second {sps}".format(ep=r.episode,
                                                                                                ts=r.timestep,
                                                                                                sps=sps))
        logger.info("Episode reward: {}".format(r.episode_rewards[-1]))
        logger.info("Episode timesteps: {}".format(r.episode_timestep))
        #logger.info("Episode largest tile: {}".format(r.environment.largest_tile))
        logger.info("Average of last 500 rewards: {}".format(sum(r.episode_rewards[-500:]) / 500))
        logger.info("Average of last 100 rewards: {}".format(sum(r.episode_rewards[-100:]) / 100))

        logger.info("Win pc of last 100 episodes: {}".format(sum(r.environment.wins[-100:]) / 100.0))

        logger.info("")
    return True

runner.run(
    timesteps=6000000,
    episodes=100000,
    max_episode_timesteps=10000,
    deterministic=False,
    episode_finished=episode_finished
)

terminal = False
state = environment.reset()
while not terminal:
    action = agent.act(state)
    state, terminal, reward = environment.execute(action)
environment.print_state()

runner.close()
