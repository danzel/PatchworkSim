from concurrent import futures
import grpc
import patchwork_pb2
import patchwork_pb2_grpc
import time

import numpy as np
import keras
from keras.layers import Input, Dense
from keras.models import Model

_ONE_DAY_IN_SECONDS = 60 * 60 * 24

# keras.backend.set_image_data_format('channels_first')

OBSERVATION_SIZE = 10 + 24 * 3

input = Input(shape=(OBSERVATION_SIZE,))

layers = input
layers = Dense(128, activation='relu')(layers)
layers = Dense(128, activation='relu')(layers)
layers = Dense(128, activation='relu')(layers)

output_winrate = Dense(64, activation='relu')(layers)
output_winrate = Dense(1, activation='tanh',
                       name='output_winrate')(output_winrate)

output_move = Dense(64, activation='relu')(layers)
output_move = Dense(4, activation='relu', name='output_move')(output_move)


model = Model(inputs=[input], outputs=[output_winrate, output_move])

model.compile(optimizer='rmsprop',
              loss={'output_winrate': 'mean_squared_error', 'output_move': 'categorical_crossentropy'})

model.predict(np.zeros((1, OBSERVATION_SIZE)))
model.fit(x = np.zeros((1, OBSERVATION_SIZE)), y={'output_winrate': np.zeros((1, 1)), 'output_move': np.zeros((1, 4))}, batch_size=32)
# print('hi')


class Server(patchwork_pb2_grpc.PatchworkServerServicer):
    def GetStaticConfig(self, request, context):
        return patchwork_pb2.StaticConfigReply(observationSize=OBSERVATION_SIZE)

    def Evaluate(self, request, context):
        # request.state[0].observation
        arr = []

        # arr = np.array([request.state[0].observation])
        for i in request.state:
            arr.append(i.observation)
        arr = np.array(arr)
        prediction = model.predict_on_batch(arr)

        # prediction[0][0][0] / 100

        evals = []
        for i in range(len(request.state)):
            evals.append(patchwork_pb2.Evaluation(winRate=prediction[0][i][0], moveRating=prediction[1][i]))

        return patchwork_pb2.EvaluateReply(evaluations=evals)

    def Train(self, request, context):

        data = []
        wins = []
        moves = []

        for sample in request.samples:
            data.append(sample.state.observation)
            wins.append([1 if sample.isWin else -1])
            moves.append(sample.moveRating)

        data = np.array(data)
        wins = np.array(wins)
        moves = np.array(moves)
        history=model.fit(x=data, y={'output_winrate': wins, 'output_move': moves}, batch_size=32)
        model.save(f'model-{time.time()}.h5')
        return patchwork_pb2.TrainReply()



def serve():
    server=grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    patchwork_pb2_grpc.add_PatchworkServerServicer_to_server(Server(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    try:
        while True:
            time.sleep(_ONE_DAY_IN_SECONDS)
    except KeyboardInterrupt:
        server.stop(0)


if __name__ == '__main__':
    serve()
