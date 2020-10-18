import json
import numpy as np

#   Supress warning and informational messages
import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3' 

from poker.model import KerasModel
from poker import converters

dimensions = (7, 4, 14)

s = ['A', 'K', 'Q', 'J', '10', '9', '8', '7', '6', '5', '4', '3', '2']





def add_data_from_file(data, inputs, outputs):
    for line in data['Lines']:
        model_input = cards_to_input(line['Input']['Cards'])
        inputs.append(model_input)
        output_value = line['Output']['Value']
        outputs.append(output_value)


def main():
    inputs = []
    outputs = []

    #files = ["test2h", "test5h", "test6h", "test7h"]

    #for file in files:
    #    data = json.load(open("./data/hand/"+file+".json"))
    #    add_data_from_file(data, inputs, outputs)

    

    model = KerasModel()
    r = model.predict(inputs)



    #model.train_model(inputs, outputs)
    #model.save_model()


if __name__ == '__main__':
    main()