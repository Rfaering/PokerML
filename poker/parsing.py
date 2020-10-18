from poker.model import KerasModel
from poker import converters

def cards_to_input(cards):
    model_input = np.zeros(dimensions)
    card_index = 0

    for card in cards:

        suit = card[0]
        value = card[1:]

        suit_index = converters.card_suit_to_index(suit) - 1 
        value_index = converters.card_value_to_index(value) - 1

        model_input[card_index, suit_index, value_index] = 1
        card_index += 1

    return model_input