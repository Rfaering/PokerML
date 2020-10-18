def card_value_to_index(value):
    return {
        'A':    14,
        'K':    13,
        'Q':    12,
        'J':    11,
        '10':   10,
        '9':    9,
        '8':    8,
        '7':    7,
        '6':    6,
        '5':    5,
        '4':    4,
        '3':    3,
        '2':    2,
    }[str(value)]

def card_suit_to_index(value):
    return {
        'S':    4,
        'H':    3,
        'D':    2,
        'C':    1,
    }[str(value)]
