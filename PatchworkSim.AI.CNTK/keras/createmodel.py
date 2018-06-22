from keras.models import Sequential, Model
from keras.layers import Input, add
from keras.layers.core import Dense, Dropout, Activation, Flatten
from keras.layers.convolutional import Conv2D, ZeroPadding2D
from keras.layers.pooling import MaxPooling2D
from keras.layers.normalization import BatchNormalization
from keras.regularizers import l2

#https://github.com/maxpumperla/betago/tree/master/betago/networks
#https://github.com/keras-team/keras/blob/master/examples/mnist_cnn.py
#https://github.com/Zeta36/connect4-alpha-zero/blob/master/src/connect4_zero/agent/model_connect4.py
#TODO: Can combine activation with layers, https://keras.io/getting-started/sequential-model-guide/ Training
def layers(input_shape):
    return [
        Conv2D(64, (3, 3), activation='relu', input_shape=input_shape, data_format='channels_first'),
        #7,7

        Conv2D(128, (3, 3), activation='relu', data_format='channels_first'),
        #5,5

        Flatten(),
        Dense(256, activation='relu'),
        Dense(256, activation='relu')
    ]


input_shape = (2, 9, 9)
model = Sequential(layers(input_shape))
model.add(Dense(1))

model.compile(loss='mse',
              optimizer='sgd')


#https://machinelearningmastery.com/save-load-keras-deep-learning-models/
# serialize model to JSON
model_json = model.to_json()
with open("model.json", "w") as json_file:
    json_file.write(model_json)
# serialize weights to HDF5
model.save_weights("model.h5")
print("Saved model to disk")

#https://github.com/Microsoft/MMdnn/blob/master/docs/keras2cntk.md


#TODO Different optimiser
#NN take previous and next state as inputs