using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public static class ElementFactory
    {
        private static Color emerald = null;
        private static Color piterRiver = null;
        private static Color amethyst = null;
        private static Color sunflower = null;
        private static Color alizarin = null;

        public static Color Emerald 
        {
            get 
            {
                if (emerald == null)
                {
                    if (App.Current.Resources.TryGetValue("Emerald", out var Emerald))
                    {
                        emerald = (Color)Emerald;
                    }
                }
                return emerald;
            } 
        }
        public static Color PiterRiver
        {
            get
            {
                if (piterRiver == null)
                {
                    if (App.Current.Resources.TryGetValue("PiterRiver", out var PiterRiver))
                    {
                        piterRiver = (Color)PiterRiver;
                    }
                }
                return piterRiver;
            }
        }
        public static Color Amethyst
        {
            get
            {
                if (amethyst == null)
                {
                    if (App.Current.Resources.TryGetValue("Amethyst", out var Amethyst))
                    {
                        amethyst = (Color)Amethyst;
                    }
                }
                return amethyst;
            }
        }
        public static Color Sunflower
        {
            get
            {
                if (sunflower == null)
                {
                    if (App.Current.Resources.TryGetValue("Sunflower", out var Sunflower))
                    {
                        sunflower = (Color)Sunflower;
                    }
                }
                return sunflower;
            }
        }
        public static Color Alizarin
        {
            get
            {
                if (alizarin == null)
                {
                    if (App.Current.Resources.TryGetValue("PastelGray1", out var Alizarin))
                    {
                        alizarin = (Color)Alizarin;
                    }
                }
                return alizarin;
            }
        }

        public static Element CreteElement(int elementTypeId)
        {
            var element = Element.GetGameObject<Element>();
            element.elementTypeId = elementTypeId;
            if (element.elementTypeId == 0)
            {
                element.color = Emerald;
                element.textIdle = "(︶︹︺)";
                element.textSelected = "(￣ヘ￣)";
                element.textFall = "(＞﹏＜)";
                element.fontSize = 16;
            }
            else if (element.elementTypeId == 1)
            {
                element.color = PiterRiver;
                element.textIdle = "(＃￣ω￣)";
                element.textSelected = "(＃￣0￣)";
                element.textFall = "(＃＞＜)";
                element.fontSize = 14;
            }
            else if (element.elementTypeId == 2)
            {
                element.color = Amethyst;
                element.textIdle = "Σ(°△°|||)︴";
                element.textSelected = "＼(º □ º l|l)/";
                element.textFall = "〣( ºΔº )〣";
                element.fontSize = 11;
            }
            else if (element.elementTypeId == 3)
            {
                element.color = Sunflower;
                element.textIdle = "ヽ(´ー` )┌";
                element.textSelected = "ヽ(ˇヘˇ)ノ";
                element.textFall = "¯\\_(ツ)_/¯";
                element.fontSize = 16;
            }
            else if (element.elementTypeId == 4)
            {
                element.color = Alizarin;
                element.textIdle = "(・_・;)";
                element.textSelected = "(・_・ヾ";
                element.textFall = "(・・ ) ?";
                element.fontSize = 16;
            }
            return element;
        }

        public static Bomb CreateBomb(int elementTypeId)
        {
            var bomb = Bomb.GetGameObject<Bomb>();
            bomb.elementTypeId = elementTypeId;
            if (bomb.elementTypeId == 0)
            {
                bomb.color = Emerald;
            }
            else if (bomb.elementTypeId == 1)
            {
                bomb.color = PiterRiver;
            }
            else if (bomb.elementTypeId == 2)
            {
                bomb.color = Amethyst;
            }
            else if (bomb.elementTypeId == 3)
            {
                bomb.color = Sunflower;
            }
            else if (bomb.elementTypeId == 4)
            {
                bomb.color = Alizarin;
            }
            bomb.textIdle = "⦿";
            bomb.textSelected = "⦿";
            bomb.textFall = "⦿";
            bomb.detonationText = "💥";
            bomb.fontSize = 38;
            bomb.readyToDetonate = null;
            return bomb;
        }

        public static Rocket CreateRocket(int elementTypeId, Direction direction)
        {
            var rocket = Rocket.GetGameObject<Rocket>();
            rocket.elementTypeId = elementTypeId;
            if (rocket.elementTypeId == 0)
            {
                rocket.color = Emerald;
            }
            else if (rocket.elementTypeId == 1)
            {
                rocket.color = PiterRiver;
            }
            else if (rocket.elementTypeId == 2)
            {
                rocket.color = Amethyst;
            }
            else if (rocket.elementTypeId == 3)
            {
                rocket.color = Sunflower;
            }
            else if (rocket.elementTypeId == 4)
            {
                rocket.color = Alizarin;
            }

            rocket.direction = direction;
            if (direction == Direction.Y)
            {
                rocket.textIdle = "⇕";
                rocket.textSelected = "⇕";
                rocket.textFall = "⇕";
                rocket.destroyerBackwards = "⇓";
                rocket.destroyerForward = "⇑";
            }
            else
            {
                rocket.textIdle = "⇔";
                rocket.textSelected = "⇔";
                rocket.textFall = "⇔";
                rocket.destroyerBackwards = "⇐";
                rocket.destroyerForward = "⇒";
            }
            rocket.fontSize = 32;
            return rocket;
        }
    }
}
