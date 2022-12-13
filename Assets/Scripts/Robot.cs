using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dance {
    public class Robot : DanceMove, IDanceMove {
        private static float initialCooldown = 1;
        private static float damage = 20;
        private readonly static Stack<KeyCode> sequence = new(new[] {KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.LeftArrow});
        public Robot() : base(sequence) {
            CurrentCooldown = 0;
            Damage = damage;
            Id = 0;
            Usable = true;
        }
        public override void SetCooldown() {
            this.CurrentCooldown = initialCooldown;
        }
    }
}