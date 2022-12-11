using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dance {
    public interface IDanceMove {
        public int Id {get; set; }
        public bool Usable {get; set;}
        public Stack<KeyCode> CurrentSequence {get; set;}
        float FullStackSize {get; set; }
        float CurrentCooldown {get; set;}
        float Damage {get; set;}
        private int GetFullStackSize() { throw new NotImplementedException(); }
        public virtual void SetCooldown() { throw new NotImplementedException(); }
        public void ResetStack(Stack<KeyCode> stack) { throw new NotImplementedException(); }
    }
}