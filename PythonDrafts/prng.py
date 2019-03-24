# prng.py

import random

random.seed(0)

statebefore = random.getstate()

for i in range(10):
    print("a",random.random())

random.setstate(statebefore)

for i in range(10):
    print("b",random.random())