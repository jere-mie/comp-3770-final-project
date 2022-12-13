using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dance {
    public abstract class DanceMove : IDanceMove {
        public int Id {get; set; }
        public bool Usable {get; set; } = false;
        private static Stack<KeyCode> sequence;
        public Stack<KeyCode> CurrentSequence {get; set;}
        public float FullStackSize {get; set; } = 0;
        public float CurrentCooldown {get; set;}
        public float Damage {get; set;} = 0;
        public DanceMove(Stack<KeyCode> seq) {
            sequence = seq;
            this.CurrentSequence = sequence;
            this.FullStackSize = GetFullStackSize();
        }
        private int GetFullStackSize() {
            return this.CurrentSequence.Count;
        }
        public virtual void SetCooldown() {
            this.CurrentCooldown = 0;
        }
        public void ResetStack(Stack<KeyCode> stack) {
            this.CurrentSequence = stack;
        }
    }
}