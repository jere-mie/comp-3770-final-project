using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dance {
    public class Headbang : DanceMove, IDanceMove {
        private static float initialCooldown = 2;
        private static float damage = 30; 
        private static readonly Stack<KeyCode> sequence = new(new[] {KeyCode.LeftArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightArrow});
        public Headbang() : base(sequence) {
            CurrentCooldown = 0;
            Damage = damage;
            Id = 2;
            Usable = true;
        }
        public override void SetCooldown() {
            this.CurrentCooldown = initialCooldown;
        }
    }
}