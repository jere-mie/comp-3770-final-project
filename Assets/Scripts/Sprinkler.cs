using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dance {
    public class Sprinkler : DanceMove, IDanceMove {
        private static float initialCooldown = 3;
        private static float damage = 40;
        private static readonly Stack<KeyCode> sequence = new(new[] {KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow});
        public Sprinkler() : base(sequence) {
            CurrentCooldown = 0;
            Damage = damage;
            Id = 1;
            Usable = true;
        }
        public override void SetCooldown() {
            this.CurrentCooldown = initialCooldown;
        }
    }
}