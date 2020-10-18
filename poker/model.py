import json
import numpy as np

import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3' 

import keras
from keras.models import Sequential
from keras.layers import Dense, Dropout, Flatten
from keras.layers import Conv2D, MaxPooling2D
from keras import backend as K

json_file_path = os.path.abspath("./data/model/model.json")
weight_file_path = os.path.abspath("./data/model/model.h5")

class KerasModel: 
    def __init__(self):
        self.load_model()

    def create_model(self, input_dimensions):
        self.model = Sequential()

        self.model.add(Conv2D(64, kernel_size=(3, 3), activation='relu', strides=1, input_shape=input_dimensions))
        self.model.add(MaxPooling2D(pool_size=(2, 2)))
        self.model.add(Flatten())
        self.model.add(Dense(128, activation='relu'))
        self.model.add(Dense(1, activation='sigmoid'))

    def load_model(self):
        json_file = open(json_file_path, 'r')
        loaded_model_json = json_file.read()
        json_file.close()

        self.model = keras.models.model_from_json(loaded_model_json)
        self.model.load_weights(weight_file_path)

    def train_model(self, inputs_train, outputs_train):
        batch_size = 100
        epochs = 100
        
        self.model.compile(loss=keras.losses.mean_squared_error, optimizer=keras.optimizers.Adam())

        self.model.summary()
        self.model.fit(np.array(inputs_train), np.array(outputs_train), batch_size=batch_size, epochs=epochs, verbose=1)

    def predict(self, inputs):
        return self.model.predict(np.array(inputs))

    def save_model(self):
        model_json = self.model.to_json()

        with open(json_file_path, "w") as json_file:
            json_file.write(model_json)

        self.model.save_weights(weight_file_path)
