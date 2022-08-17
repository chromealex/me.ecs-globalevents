using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ME.ECS.GlobalEvents {

    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
    #endif
    public static class WorldInitializer {

        public static DisposeStatic disposeStatic = new DisposeStatic();
        
        private static bool initialized = false;
        
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        private static class EditorInitializer {
            static EditorInitializer() => WorldInitializer.Initialize();
        }
        #endif

        [UnityEngine.RuntimeInitializeOnLoadMethodAttribute(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            
            if (WorldInitializer.initialized == false) {
                
                WorldStaticCallbacks.RegisterCallbacks(InitWorld, DisposeWorld);
                WorldStaticCallbacks.RegisterCallbacks(OnWorldStep);
                WorldStaticCallbacks.RegisterCallbacks(InitResetState);

                WorldInitializer.initialized = true;
            }
            
        }

        public class DisposeStatic {
            ~DisposeStatic() {
                WorldStaticCallbacks.UnRegisterCallbacks(InitWorld, DisposeWorld);
                WorldStaticCallbacks.UnRegisterCallbacks(OnWorldStep);
                WorldStaticCallbacks.UnRegisterCallbacks(InitResetState);
            }
        }

        public static void InitResetState(State state) {

            state.pluginsStorage.GetOrCreate<GlobalEventStorage>(ref state.allocator);

        }

        public static void InitWorld(World world) {

            world.GetNoStateData().pluginsStorage.GetOrCreate<WorldStorage>(ref world.GetNoStateData().allocator);

        }
        
        public static void DisposeWorld(World world) {
            
        }

        public static void OnWorldStep(World world, WorldCallbackStep step) {

            if (step == WorldCallbackStep.LogicTick) {

                world.ProcessGlobalEvents(GlobalEventType.Logic);

            } else if (step == WorldCallbackStep.VisualTick) {
            
                world.ProcessGlobalEvents(GlobalEventType.Visual);

            }
            
        }

    }

}