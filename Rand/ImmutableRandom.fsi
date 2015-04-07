namespace Rand

type ImmutableRandom

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableRandom =
    open System

    val fromSeed : int -> ImmutableRandom
    val int : ImmutableRandom -> int
    val intInRange : ImmutableRandom -> int -> int -> int
    val double : ImmutableRandom -> double
    val next : ImmutableRandom -> ImmutableRandom