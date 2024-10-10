namespace Bits

module Diamond =
    // let private letters upTo =
    //         ['a'..upTo]
    //
    // let mirrored s =
    //     s @ (s |> List.rev |> List.skip 1)
    //
    // let vertMirrored (xs: 'a list) : 'a list =
    //     xs @ (xs |> List.rev |> List.tail)
    //
    let stringify (cs: char seq seq) =
        cs
        |> Seq.map System.String.Concat
        |> String.concat "\r\n"
            
        
    
    let diamond (upTo: char) : string =
        [] |> stringify
