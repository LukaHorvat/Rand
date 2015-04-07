namespace Rand

type 'a Gen = Gen of (ImmutableRandom -> ImmutableRandom * 'a)

module InternalGen =
    open ImmutableRandom
    let fromGenerator f = Gen(fun rand -> (next rand, f rand))
    let constant a = fromGenerator (fun _ -> a)
    let bind (Gen ra) agb =
        let rrb r = 
            let (newR, a) = ra r
            let (Gen(rb)) = agb a
            rb newR
        Gen(rrb)
    let runGen (Gen f) rand = f rand |> snd 
        
[<Class>]
[<Sealed>]
type GenBuilder() =
    member x.Bind (g, f) = InternalGen.bind g f
    member x.Return a = InternalGen.constant a
    member x.ReturnFrom (g : 'a Gen) = g

[<AutoOpen>]
module GenBuild =
    let gen = GenBuilder()

module Gen =
    open System
    open ImmutableRandom
    open Microsoft.FSharp.Reflection

    let fromGenerator f = InternalGen.fromGenerator f
    let constant a = InternalGen.constant a
    let bind g f = InternalGen.bind g f
    let runGen g rand = InternalGen.runGen g rand
    let (>>=) g f = bind g f
    let map f g = g >>= (f >> constant)
    let (<!>) f g = map f g
    let ap gf ga = gen {
        let! f = gf
        let! a = ga
        return f a
    }
    let (<*>) gf ga = ap gf ga
    let join gga = gga >>= id
    let rec sequence gs =
        match gs with
        | []      -> constant []
        | x :: xs -> gen {
            let! this = x
            let! rest = sequence xs 
            return this :: rest 
        }
    let intTo n = fromGenerator (fun rand -> intInRange rand 0 n)
    let oneOf gs = gen { 
        let! n = intTo (List.length gs - 1)
        return! List.nth gs n 
    } 
    let elementOf xs = List.map constant xs |> oneOf
    let listOf g n = List.replicate n g |> sequence
    let alternative g1 g2 = oneOf [g1; g2]
    let (<|>) g1 g2 = alternative g1 g2

module GenOps =
    open Gen
    
    let (>>=) g f = bind g f
    let (<!>) f g = map f g
    let (<*>) gf ga = ap gf ga
    let (<|>) g1 g2 = alternative g1 g2

type 'a Gen with
    member g.Map() = Gen.map (fun a -> a :> obj) g