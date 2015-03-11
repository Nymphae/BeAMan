using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using System;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Microsoft.Win32.SafeHandles;
using Color = System.Drawing.Color;

namespace Be_A_Man
{
    internal class Program
    {

        private static Obj_AI_Hero Player = ObjectManager.Player;
        private static String championName = "XinZhao";
        private static Menu _Menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu TSMenu;
        private static Spell Q, W, E, R;


        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            {
                if (Player.ChampionName != championName)

                    return;
            }
            Q = new Spell(SpellSlot.Q, 175);
            W = new Spell(SpellSlot.W, 40);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 350);



            _Menu = new Menu("ManZhao", "xin.mainmenu", true);
            TSMenu = new Menu("Target Selector", "target.selector");
            TargetSelector.AddToMenu(TSMenu);
            _Menu.AddSubMenu(TSMenu);
            _orbwalker = new Orbwalking.Orbwalker(_Menu.SubMenu("Orbwalking"));
            var comboMenu = new Menu("Combo", "kek.xin.combo");
            {
                comboMenu.AddItem(new MenuItem("kek.xin.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.xin.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.xin.combo.usee", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.xin.combo.user", "Auto use R (change settings below").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.xin.combo.ecounter", "Dont E if X Enemies are present").SetValue(new Slider(1, 1, 5)));
                comboMenu.AddItem(new MenuItem("kek.xin.combo.rcount", "Ult if X Enemies are preset").SetValue(new Slider(1, 1, 5)));
            }

            var farmMenu = new Menu("Jungle/Lane Clear", "kek.xin.farm");
            {
                farmMenu.AddItem(new MenuItem("kek.xin.farm.farmq", "Use Q to Farm").SetValue(true));
                farmMenu.AddItem(new MenuItem("kek.xin.farm.farme", "use E to farm").SetValue(true));
                farmMenu.AddItem(new MenuItem("kek.xin.farm.farmw", "Use W to Farm").SetValue(true));
            }

            var ksMenu = new Menu("Kill Secure", "kek.xin.ks");
            {
                ksMenu.AddItem(new MenuItem("kek.xin.ks.EQ", "Use EQ Combo to KS").SetValue(true));
                ksMenu.AddItem(new MenuItem("kek.xin.ks.R", "Use R to KS").SetValue(true));
            }

            var drawMenu = new Menu("Draw Settings", "kek.xin.draw");
            {
                drawMenu.AddItem(new MenuItem("kek.xin.drawRrange", "R Draw").SetValue(true));
                drawMenu.AddItem(new MenuItem("kek.xin.drawErange", "E Draw").SetValue(true));
            }

            _Menu.AddSubMenu(ksMenu);
            _Menu.AddSubMenu(drawMenu);
            _Menu.AddSubMenu(farmMenu);
            _Menu.AddSubMenu(comboMenu);
            _Menu.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            LeagueSharp.Drawing.OnDraw += Drawing;
            Game.PrintChat("Xin Loaded. Be A Fucking Man.");


        }

        private static void Drawing(EventArgs args)
        {
            if (_Menu.Item("kek.xin.drawErange").GetValue<bool>())
            {
                if (E.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.DarkCyan);

                }

            }
            if (_Menu.Item("kek.xin.drawRrange").GetValue<bool>())
            {
                if (R.IsReady())
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color.LightYellow);

                }

            }

        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear();
            }
        }


        private static void JungleClear()
        {
            var creeps = MinionManager.GetMinions(
                Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (creeps.Count > 0)
            {
                var minions = creeps[0];
                if (_Menu.Item("kek.xin.farm.farme").GetValue<bool>() && E.IsReady() && minions.IsValidTarget(E.Range))
                {
                    E.Cast(minions);
                }

                if
                   (_Menu.Item("kek.xin.farm.farmw").GetValue<bool>() && W.IsReady() && minions.IsValidTarget(Q.Range))
                {
                    W.Cast();
                }

                if (_Menu.Item("kek.xin.farm.farmq").GetValue<bool>() && !E.IsReady() && !W.IsReady() &&
                    minions.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

            }
        }

        private static void Combo()
        {
        var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            var Esetting = _Menu.Item("kek.xin.combo.ecounter").GetValue<Slider>().Value;
            var Rset = _Menu.Item("kek.xin.combo.rcount").GetValue<Slider>().Value;

            if (target !=null && _Menu.Item("kek.xin.combo.usee").GetValue<bool>() && E.IsReady())
            {
               if (eTarget.CountEnemiesInRange(1000f) < Esetting)
                {
                    E.Cast(target);
                    
                }
            }

            if (target!= null && _Menu.Item("kek.xin.combo.usew").GetValue<bool>() && W.IsReady())
            {
                W.Cast();
            }
            
           if (target != null && _Menu.Item("kek.xin.combo.useq").GetValue<bool>() && !E.IsReady() && !W.IsReady() && Q.IsReady())
            {
                Q.Cast();
            }

            
          if (_Menu.Item("kek.xin.ks.R").GetValue<bool>())
            {
                KillstealR();
            }

            if (_Menu.Item("kek.xin.ks.EQ").GetValue<bool>())
            {
                KillstealEQ();
            }
            if ( target !=null && _Menu.Item("kek.xin.combo.user").GetValue<bool>() &&  R.IsReady())
            {
                if (rTarget.CountEnemiesInRange(1000f) >= Rset)

                {
                    R.Cast();
                }
            }
        }


        private static void KillstealR()
        {

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Range)))
            {
                if (R.IsReady() && hero.Distance(ObjectManager.Player) <= R.Range &&
                    Player.GetSpellDamage(hero, SpellSlot.R) >= hero.Health)
                    R.Cast();
            }


        }

        private static void KillstealEQ()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(E.Range)))
            {
                double DamageE = Player.GetSpellDamage(hero, SpellSlot.E);
                double DamageQ = Player.GetSpellDamage(hero, SpellSlot.Q);
                double DamageTotal = DamageQ + DamageE;
                if (E.IsReady() && hero.Distance(ObjectManager.Player) <= E.Range && DamageTotal >= hero.Health)
                    Q.Cast();
                E.Cast();
            }
        }

    }

}
