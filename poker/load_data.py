import json 
import numpy as np

def num_to_card_value(value):
    return {
        '14.0': 'A',
        '13.0': 'K',
        '12.0': 'Q',
        '11.0': 'J',
        '10.0': '10',
        '9.0': '9',
        '8.0': '8',
        '7.0': '7',
        '6.0': '6',
        '5.0': '5',
        '4.0': '4',
        '3.0': '3',
        '2.0': '2',        
    }[str(value)]

def load_file():
    json_data = json.load(open('./data/input/test.json', 'r'))
    training_data = json_data['TrainingData']
    
    result = []
    
    index_before_values = 5

    for first_row in training_data:
        input = first_row['Input']
        output = first_row['Output']
        maxValue = max(output)
        
        column_num_value = input[index_before_values+2]
        row_num_value = input[index_before_values+4]
        
        new_value = { 
            'column_display': num_to_card_value(column_num_value), 
            'column_sort': (14-column_num_value), 
            'row_display': num_to_card_value(row_num_value), 
            'row_sort': (14-row_num_value), 
            'value': maxValue }

        result.append(new_value)        

    json_data = json.dump(result, open('./data/output/test.json', 'w'))

    #a = np.zeros((5, 16, 16))

    #for i in range(5):
    #    offset = 6
    #    suit = offset + i * 2
    #    value = offset + i * 2 + 1
    #    a[i][suit-1][value-1] = 1

    #a = np.zeros((3, 16, 16))
    #print(a[0])

load_file()


