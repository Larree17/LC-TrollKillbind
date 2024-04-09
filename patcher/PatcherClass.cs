using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using MonoMod.Utils;
using Mono.Cecil.Cil;

namespace patcher
{
    public class PatcherClass
    {
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        public static void Patch(AssemblyDefinition assembly)
        {
            var a = BepInEx.Logging.Logger.CreateLogSource("hi");
            a.LogInfo("testing");
            var origB = assembly.MainModule.GetType("GameNetworkManager");

            var GAMEVERSION = origB.FindField("gameVersionNum");
            var cctor = origB.Methods.SingleOrDefault(x => x.Name == "Start");
            if (cctor == null) { a.LogInfo("is null"); }

            var store = cctor.Body.Instructions;
            foreach (Instruction ins in store)
            {
                a.LogInfo(ins);
                if (ins.ToString() == "ldfld System.Int32 GameNetworkManager::gameVersionNum") { a.LogInfo(ins.GetIntOrNull()); }
            }

            a.LogInfo("done");
            // Patcher code here
        }
    }
}