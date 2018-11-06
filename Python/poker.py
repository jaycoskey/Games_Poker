#!/usr/bin/env python

from collections import Counter
from enum import auto, Enum
from random import randint
import sys
from time import sleep
from typing import Iterable


#
# Exceptions
#
class EmptyDeckException(Exception):
    pass


#
# Enumerations
#
class HandType(Enum):
    ROYAL_FLUSH = 9
    STRAIGHT_FLUSH = 8
    FOUR_OF_A_KIND = 7
    FULL_HOUSE = 6
    FLUSH = 5
    STRAIGHT = 4
    THREE_OF_A_KIND = 3
    TWO_PAIR = 2
    PAIR = 1
    HIGH_CARD = 0


#
# Classes
#
class Card:
    MIN_VALUE = 2
    MAX_VALUE = 14

    def __init__(self, suit:'Suit', value:int):
        assert(Card.MIN_VALUE <= value <= Card.MAX_VALUE)
        self.suit = suit
        self.value = value

    def _value_repr(self):
        if 2 <= self.value <= 10:
            return str(self.value)
        elif self.value == 10:
            return '10'
        elif self.value == 11:
            return 'J'
        elif self.value == 12:
            return 'Q'
        elif self.value == 13:
            return 'K'
        elif self.value == 14:
            return 'A'
        else:
            raise ValueError(f'Unrecognized card value: {self.value}')

    def __repr__(self):
        return f'{self._value_repr()}{Suit.tostr(self.suit)}'


class Deck:
    def __init__(self):
        self.deck = [Card(suit, value)
                        for suit in [Suit.C, Suit.D, Suit.H, Suit.S]
                        for value in range(Card.MIN_VALUE, Card.MAX_VALUE + 1)
                    ]

    def __iter__(self):
        yield from self.deck

    def deal(self, num_hands, num_cards=5):
        hands = [[] for _ in range(num_hands)]
        for _ in range(num_cards):
            for k in range(num_hands):
                hands[k].append(self.draw())
        return list(map(lambda cs: Hand(cs), hands))

    def draw(self):
        if len(self.deck) == 0:
            raise EmptyDeckException()
        pos = randint(0, len(self.deck) - 1)
        return self.deck.pop(pos)

    def print(self):
        print(self.deck)


class Hand:
    ACE_LOW_STRAIGHT_VALUES = [14, 2, 3, 4, 5]

    def __init__(self, cards:Iterable[Card]):
        self.cards = cards

    def __repr__(self):
        return repr(self.cards)

    def rank_5(self):
        """Return the rank of a 5-card hand"""
        suits = [c.suit for c in self.cards]
        num_suits = len(set(suits))

        values = [c.value for c in self.cards]
        num_values = len(set(values))
        max_value = max(values)
        span_values = max_value - min(values)
        vcntr = Counter(values)
        vcntr_values = sorted(vcntr.keys(), key=lambda k: vcntr[k], reverse=True)

        is_straight = num_values == 5 and span_values == 4
        is_flush = num_suits == 1
        is_straight_flush = is_flush and is_straight
        is_royal_flush = is_straight_flush and max_value == 13

        if is_royal_flush:
            return HandRank(HandType.ROYAL_FLUSH, [max_value])

        if is_straight_flush:
            return HandRank(HandType.STRAIGHT_FLUSH, [max_value])

        vcntr_counts = sorted(vcntr.values(), reverse=True)
        if vcntr_counts[0] == 4:
            other_values = [c.value for c in self.cards if c.value != vcntr_values[0]]
            return HandRank(HandType.FOUR_OF_A_KIND, [vcntr_values[0], other_values[0]])

        if vcntr_counts[0:2] == [3, 2]:
            return HandRank(HandType.FULL_HOUSE, vcntr_values[0:2])

        values_desc = sorted(values, reverse=True)
        if is_flush:
            return HandRank(HandType.FLUSH, values_desc)

        if is_straight:
            return HandRank(HandType.STRAIGHT, [max_value])

        is_ace_low_straight = num_values == 5 and all([v in values for v in Hand.ACE_LOW_STRAIGHT_VALUES])
        if is_ace_low_straight:
            return HandRank(HandType.STRAIGHT, [5])

        if vcntr_counts[0] == 3:
            other_values = [c.value for c in self.cards if c.value != vcntr_values[0]]
            other_values_desc = sorted(other_values, reverse=True)
            return HandRank(HandType.THREE_OF_A_KIND, [vcntr_values[0]] + other_values_desc)

        if vcntr_counts[0:2] == [2, 2]:
            card_ranks = sorted(vcntr_values[0:2], reverse=True)
            other_values = [c.value for c in self.cards if c.value != vcntr_values[0:2]]
            return HandRank(HandType.TWO_PAIR, card_ranks + [other_values[0]])

        if vcntr_counts[0] == 2:
            return HandRank(HandType.PAIR, [vcntr_values[0]])

        return HandRank(HandType.HIGH_CARD, values_desc)

    def rank_n(self):
        """Determine the rank of a hand with 5+ cards"""
        # if True:
        #     return HandRank(HandType.ROYAL_FLUSH, [...])
        # if True:
        #     return HandRank(HandType.STRAIGHT_FLUSH, [...])
        # if True:
        #     return HandRank(HandType.FOUR_OF_A_KIND, [...])
        # if True:
        #     return HandRank(HandType.FULL_HOUSE, [...])
        # if True:
        #     return HandRank(HandType.FLUSH, [...])
        # if True:
        #     return HandRank(HandType.STRAIGHT, [...])
        # if True:
        #     return HandRank(HandType.THREE_OF_A_KIND, [...])
        # if True:
        #     return HandRank(HandType.TWO_PAIR, [...])
        # if True:
        #     return HandRank(HandType.PAIR, [...])
        # return HandRank(HandType.HIGH_CARD, [...])
        pass  # TODO


class HandRank:
    def __init__(self, hand_type, values):
       self.hand_type = hand_type
       self.values = values

    def __gt__(self, other):
        if self.hand_type.value > other.hand_type.value:
            return True
        elif self.hand_type.value == other.hand_type.value:
            return self.values > other.values
        else:
            return False

    def __repr__(self):
        htr = self._hand_type_repr()
        return f'{htr}{repr(self.values)}'

    def _hand_type_repr(self):
        ht_reprs = { HandType.ROYAL_FLUSH: 'Royal Flush'
                , HandType.STRAIGHT_FLUSH: 'Straight flush'
                , HandType.FOUR_OF_A_KIND: 'Four of a kind'
                , HandType.FULL_HOUSE: 'Full house'
                , HandType.FLUSH: 'Flush'
                , HandType.STRAIGHT: 'Straight'
                , HandType.THREE_OF_A_KIND: 'Three of a kind'
                , HandType.TWO_PAIR: 'Two pair'
                , HandType.PAIR: 'Pair'
                , HandType.HIGH_CARD: 'High card'
                }
        return ht_reprs[self.hand_type]


class Suit(Enum):
    C = auto()
    D = auto()
    H = auto()
    S = auto()

    @staticmethod
    def tostr(suit):
        if suit == Suit.C: return 'C'
        elif suit == Suit.D: return 'D'
        elif suit == Suit.H: return 'H'
        elif suit == Suit.S: return 'S'
        else: raise ValueError('Unrecognized card suit')


def print_high_hands():
    while True:
        deck = Deck()
        hand = Hand([deck.draw() for _ in range(5)])
        rank = hand.rank_5()
        if rank.hand_type.value > HandType.FULL_HOUSE.value:
            print(f'{hand}: {rank}')


if __name__ == '__main__':
    while True:
        deck = Deck()
        hand1, hand2 = deck.deal(num_hands=2)
        rank1 = hand1.rank_5()
        rank2 = hand2.rank_5()
        if not (rank1.hand_type.value > HandType.THREE_OF_A_KIND.value and rank2.hand_type.value > HandType.THREE_OF_A_KIND.value):
            continue
        if rank2 > rank1:
            hand1, hand2 = hand2, hand1
            rank1, rank2 = rank2, rank1
        print('{:25s} ({:25s}) >= {:25s} ({:25s})'.format(str(hand1), str(rank1), str(hand2), str(rank2)))

