from keras.models import Sequential
from keras.layers.core import Dense, Dropout, Activation, Flatten
from keras.layers.convolutional import Conv2D, ZeroPadding2D

#https://github.com/maxpumperla/betago/tree/master/betago/networks
#https://github.com/keras-team/keras/blob/master/examples/mnist_cnn.py
#TODO: Can combine activation with layers, https://keras.io/getting-started/sequential-model-guide/ Training
def layers(input_shape):
    return [
        ZeroPadding2D((2, 2), input_shape=input_shape),
        Conv2D(64, (5, 5), activation='relu'),

        ZeroPadding2D((1, 1)),
        Conv2D(32, (3, 3), activation='relu'),

        ZeroPadding2D((1, 1)),
        Conv2D(32, (3, 3), activation='relu'),

        ZeroPadding2D((1, 1)),
        Conv2D(32, (3, 3), activation='relu'),

        Flatten(),
        Dense(256, activation='relu')
    ]


input_shape = (9, 9, 1)
nb_classes = 1 + 9 * 9 #estimated final coverage, more is better

model = Sequential(layers(input_shape))
model.add(Dropout(0.5))
model.add(Dense(nb_classes))
model.add(Activation('softmax'))

model.compile(loss='categorical_crossentropy',
              optimizer='adadelta',
              metrics=['accuracy'])


#https://machinelearningmastery.com/save-load-keras-deep-learning-models/
# serialize model to JSON
model_json = model.to_json()
with open("model.json", "w") as json_file:
    json_file.write(model_json)
# serialize weights to HDF5
model.save_weights("model.h5")
print("Saved model to disk")

#https://github.com/Microsoft/MMdnn/blob/master/docs/keras2cntk.md
