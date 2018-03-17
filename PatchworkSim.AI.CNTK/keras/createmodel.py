from keras.models import Sequential, Model
from keras.layers import Input, add
from keras.layers.core import Dense, Dropout, Activation, Flatten
from keras.layers.convolutional import Conv2D, ZeroPadding2D
from keras.layers.pooling import MaxPooling2D
from keras.layers.normalization import BatchNormalization

#https://github.com/maxpumperla/betago/tree/master/betago/networks
#https://github.com/keras-team/keras/blob/master/examples/mnist_cnn.py
#TODO: Can combine activation with layers, https://keras.io/getting-started/sequential-model-guide/ Training
def layers(input_shape):
    return [
        ZeroPadding2D((1, 1), input_shape=input_shape),
        Conv2D(128, (3, 3)),
        BatchNormalization(),
        Activation('relu'),

        ZeroPadding2D((1, 1)),
        Conv2D(32, (3, 3), activation='relu'),
        #MaxPooling2D(),
        ZeroPadding2D((1, 1)),
        Conv2D(32, (3, 3), activation='relu'),
        ZeroPadding2D((1, 1)),
        Conv2D(32, (3, 3), activation='relu'),

        Flatten(),
        Dense(256, activation='relu')
    ]


input_shape = (9, 9, 1)
nb_classes = 2 #% likelyhood this is a good placement

#Input
i = Input(shape=input_shape)

#First conv layer
c = ZeroPadding2D((1, 1))(i)
c = Conv2D(128, (3, 3))(c)
c = BatchNormalization()(c)
c = Activation('relu')(c)

#residual layer
r = ZeroPadding2D((1, 1))(c)
r = Conv2D(128, (3, 3))(r)
r = BatchNormalization()(r)

r = add([i, r])
r = Activation('relu')(r)

#residual layer
r = ZeroPadding2D((1, 1))(r)
r = Conv2D(128, (3, 3))(r)
r = BatchNormalization()(r)

r = add([i, r])
r = Activation('relu')(r)

#residual layer
r = ZeroPadding2D((1, 1))(r)
r = Conv2D(128, (3, 3))(r)
r = BatchNormalization()(r)

r = add([i, r])
r = Activation('relu')(r)

r = Flatten()(r)
r = Dense(256, activation='relu')(r)
r = Dropout(0.5)(r)
r = Dense(nb_classes)(r)
r = Activation('softmax')(r)

model = Model(i, r)

#model = Sequential(layers(input_shape))
#model.add(Flatten())
#model.add(Dense(256, activation='relu'))
#model.add(Dropout(0.5))
#model.add(Dense(nb_classes))
#model.add(Activation('softmax'))

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
