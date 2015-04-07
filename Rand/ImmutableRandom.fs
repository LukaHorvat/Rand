namespace Rand

type ImmutableRandom = ImmutableRandom of int * Lazy<ImmutableRandom>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ImmutableRandom =
    open System

    let rand n = Random(n).Next()

    let rec fromSeed n = ImmutableRandom(n, lazy (rand n |> fromSeed))
    let int (ImmutableRandom(n, _)) = rand n
    let intInRange (ImmutableRandom(n, _)) min max = Random(n).Next(min, max + 1)
    let double (ImmutableRandom(n, _)) = Random(n).NextDouble()
    let next (ImmutableRandom(_, lr)) = lr.Force() 
