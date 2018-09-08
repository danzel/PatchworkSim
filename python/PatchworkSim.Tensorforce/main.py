import os
os.environ["CUDA_VISIBLE_DEVICES"]="-1"    

import logging
import time
import grpc
import patchwork_pb2
import patchwork_pb2_grpc
from tensorforce.agents import PPOAgent
from tensorforce.environments import Environment
from tensorforce.execution import Runner

opponent_strength = 100

total_time = 0
reset_time = 0
execute_time = 0

class PatchworkEnv(Environment):
    def __init__(self):
        self.channel = grpc.insecure_channel('localhost:50900')
        self.stub = patchwork_pb2_grpc.PatchworkServerStub(self.channel)
        self.obs_size = self.stub.GetStaticConfig(patchwork_pb2.StaticConfigRequest())
        print(f'obs_size is {self.obs_size}')
        self.wins = []

    def reset(self):
        global reset_time
        start = time.perf_counter()

        res = self.stub.Create(patchwork_pb2.CreateRequest(opponentStrength = opponent_strength))
        self.gameId = res.gameId

        reset_time += (time.perf_counter() - start)

        return res.observation.observationForNextMove

    def execute(self, action):
        global execute_time
        start = time.perf_counter()

        move = int(action)
        res = self.stub.PerformMove(patchwork_pb2.MoveRequest(gameId = self.gameId, move = move))

        if res.gameHasEnded:
            self.wins.append(1 - res.winningPlayer)

        execute_time += (time.perf_counter() - start)

        return (res.observation.observationForNextMove, res.gameHasEnded, res.observation.reward)

    @property
    def states(self):
        return dict(shape=(self.obs_size.observationSize,), type='float')

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
        dict(type='dense', size=256),
        dict(type='dense', size=256),
        dict(type='dense', size=256)
    ],
    update_mode=dict(
        unit= 'episodes',
        batch_size=10
    ),
    # PGModel
    baseline_mode='states',
    baseline=dict(
        type='mlp',
        sizes=[256, 256, 256]
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

agent.restore_model('./models', 'net-297000-0.57-5126573')

runner = Runner(
    agent=agent,
    environment=environment
)

start_time = time.perf_counter()

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

        winpc = sum(r.environment.wins[-100:]) / 100.0
        logger.info("Win pc of last 100 episodes: {}".format(winpc))
        logger.info("Total Time {total}, in sim {sim}".format(total = (time.perf_counter() - start_time), sim = (reset_time + execute_time)))
        logger.info("")
        if r.episode % 1000 == 0:
            filename = f'net-{r.episode}-{winpc}'
            logger.info('saving to {}'.format(filename))
            agent.save_model(f'./models2/{filename}')
    return True

runner.run(
    timesteps=600000000,
    episodes=1000000,
    max_episode_timesteps=1000000,
    deterministic=False,
    episode_finished=episode_finished
)

#terminal = False
#state = environment.reset()
#while not terminal:
#    action = agent.act(state)
#    state, terminal, reward = environment.execute(action)
#environment.print_state()

runner.close()
