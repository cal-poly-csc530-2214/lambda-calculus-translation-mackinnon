## Lambda Calculus to C# Translator

This project translates a lambda calculus (LC) program into a single, valid C# expression.

### Overview
The program reads LC either from an input file or from standard in. From there, it parses the program, generating an abstract syntax tree (AST) representing the program. Then, the type inferrer processes the AST and generates type information for each node in the tree. If the type inferrence succeeds (more on that later), the AST is translated into "typed LC" and C#, with both translations outputted to standard out. The user may then optionally run the generated C# expression by typing 'y' when prompted.

### Type Inferrence
Easily the most interesting challenge of this project was the type inferrence mechanism. Being a typed language, C# requires that anonymous functions declare their argument and return types explicitly. For example, here is a lambda calculus program and its equivalent C# expression:

```
LC:
(((/ x => x)
 ((/ y => y)
  (/ z => z))) 42)

C#:
new Func<Func<int, int>, Func<int, int>>(x => x)(
    new Func<Func<int, int>, Func<int, int>>(y => y)(
        new Func<int, int>(z => z)))(42)
```

As such, in order to properly generate C# types, we need a way to infer types from the original LC. In many cases, we can do this easily. Consider the LC program `5`. Clearly, we can conclude that the type of the equivalent C# program is `int` (assuming we're treating all numbers as integers). However, consider the LC program `(/ x => x)`. What is the type of `x`? Further, what is the type of the entire program?

Turns out, this program can't be translated to C#. We can't infer the type of the program, since the lambda is never applied to a value. However, notice that the LC program `((/ x => x) 5)` _can_ have its type inferred based on the fact that we're passing a number to the lambda as `x`.

Conceptually, my type inferrer works by initially assigning every node in the AST with an `undefined` type, then working its way down the AST from the top. At each level, there is an "expected" return type, and the current AST node is expected to match that return type. If the types can't be matched, this signals a type error. For example, in the program `(/ x => (+ x 1))`, we can infer the type of `x` because the `+` expression expects both its operands to be numbers.

However, a pure top-down type inferrence approach isn't always sufficient. Consider the following LC program:

```
(((/ x => x)
  (/ y => y)) 42)
```

The type of the program can't be inferred until `(/ x => x)` is inferred, which can't be inferred until `x` is inferred, which can't be inferred until `(/ y => y)` is inferred, which can't be inferred until `y` is inferred from the value `42`. Thus, as the type inferrer works its way back up the AST from inferring the type of `y`, it needs to update both the return and argument types of each lambda expression. So, my type inferrer works its way all the way down the AST, and then all the way back up in order to find the complete type of the program. If there are any AST nodes with `undefined` types, the translation will fail because part of the program was ambiguous in type.

The less interesting aspects of type inferrence include managing variable scopes and identifying when a variable is used in two incompatible ways type-wise, so I'm not going to talk about that here. The type inferrence code is located in `LCTranslator/Analysis/TypeInferrer.cs`.
