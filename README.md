C# bits
=========


## Zero gap
Find the longest sequence of `0`s surrounded by `1`s.

e.g.

`100011111001`  should return `3`.

Leading and training `0`s should be ignored, as they are not surrounded both sides.

[ZeroGapTest.cs](https://github.com/arialdomartini/csharp-bits/blob/master/src/CSharpBits.Test/ZeroGapTest.cs)

## Unroll recursion
C# has no tail recursion optimization. Write a function that takes the elements (lambdas) of a generic recursive algorithm and unroll it so it is executed iteratively.

[RecursionUnroll.cs](https://github.com/arialdomartini/csharp-bits/blob/master/src/CSharpBits.Test/RecursionUnroll.cs)


## Permutations
Calculate all the possible permutations of a string, both itertively and recursively

[PermutationsTest.cs](https://github.com/arialdomartini/csharp-bits/blob/master/src/CSharpBits.Test/PermutationsTest.cs)

## Cascade of ifs
Remove a convoluted code full of if statements by using Chain of Responsibility

[Cascade of IFs](https://github.com/arialdomartini/csharp-bits/tree/master/src/CSharpBits/CascadeOfIfs)


## gRPC - self-hosting and invocation from a xUnit test

[xUnit test](https://github.com/arialdomartini/csharp-bits/blob/master/src/CSharpBits.Test/SampleGrpc/GreeterClientTest.cs)
