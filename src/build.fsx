#r "packages/FAKE/tools/FakeLib.dll" 
open Fake 
open Fake.Testing.XUnit2
let buildDir = "./build" 
let xunitExe = @".\packages\xunit.runner.console\tools\xunit.console.exe"
let testsDir = "./BuildTests"

Target "Clean" (fun _ -> CleanDir buildDir) 
Target "BuildApp" (fun _ ->          
        !! "**/*.fsproj"            
        -- "**/*Tests.fsproj"            
        |> MSBuildRelease buildDir "Build"            
        |> Log "AppBuild-Output: " ) 

Target "BuildUnitTests" (fun _ -> 
        !! "**/*Tests.fsproj"
        |> MSBuildDebug testsDir "Build" 
        |> Log "TestBuild-Output: " ) 
        
Target "RunUnitTests" (fun _ -> 
        !! (testsDir @@ "**/*Tests*.dll")
        |> xUnit2 (fun p -> p ))

"Clean"  ==> 
    "BuildApp" ==>
    "BuildUnitTests" ==>
    "RunUnitTests"
     

RunTargetOrDefault "RunUnitTests"