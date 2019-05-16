C# bits
=========


## Zero gap
Find the longest sequence of `0`s surrounded by `1`s.

e.g.

`100011111001`  should return `3`.

Leading and training `0`s should be ignored, as they are not surrounded both sides.


## Unroll recursion
C# has no tail recursion optimization. Write a function that takes the elements (lambdas) of a generic recursive algorithm and unroll it so it is executed iteratively.
