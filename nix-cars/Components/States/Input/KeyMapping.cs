using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.States.Input
{
    public class KeyMappings
    {
        public Key Enter;
        public Key Escape;

        public Key Forward;
        public Key Backward;
        public Key Left;
        public Key Right;
        public Key Forward2;
        public Key Backward2;
        public Key Left2;
        public Key Right2;
        public Key Reset;
        public Key Fire;
        public Key Boost;
        
        public Key Ability1;
        public Key Ability2;
        public Key Ability3;
        public Key Ability4;

        public List<Key> MappedKeys;

        public Key TAB, CAPS;
        public Key Command;
        
        public Key ConvertKey(Keys key)
        {
            //Keys.F20 = MB1
            //Keys.F21 = MB2
            //Keys.F22 = MB3
            //Keys.F23 = Scroll Down
            //Keys.F24 = Scroll Up

            if (key == Keys.F20)
            {
                return new MouseKey(MouseButton.Left);
            }
            if (key == Keys.F21)
            {
                return new MouseKey(MouseButton.Right);
            }
            if (key == Keys.F22)
            {
                return new MouseKey(MouseButton.Middle);
            }
            if (key == Keys.F23)
            {
                return new ScrollWheel(true);
            }
            if (key == Keys.F24)
            {
                return new ScrollWheel(false);
            }
            
            return new KeyboardKey(key);
        }
        public KeyMappings(JsonKeys keys)
        {
            Enter = ConvertKey(keys.KeyEnter);
            Escape = ConvertKey(keys.KeyEscape);

            Forward = ConvertKey(keys.KeyForward);
            Backward = ConvertKey(keys.KeyBackward);
            Left = ConvertKey(keys.KeyLeft);
            Right = ConvertKey(keys.KeyRight);
            Forward2 = ConvertKey(keys.KeyForward2);
            Backward2 = ConvertKey(keys.KeyBackward2);
            Left2 = ConvertKey(keys.KeyLeft2);
            Right2 = ConvertKey(keys.KeyRight2);

            Reset = ConvertKey(keys.KeyReset);
            Fire = ConvertKey(keys.KeyFire);
            Boost = ConvertKey(keys.KeyBoost);
            
            Ability1 = ConvertKey(keys.Ability1);
            Ability2 = ConvertKey(keys.Ability2);
            Ability3 = ConvertKey(keys.Ability3);
            Ability4 = ConvertKey(keys.Ability4);


            MappedKeys = new List<Key>();
            MappedKeys.AddRange(new List<Key>()
            {
                Enter, Escape,
                Forward, Backward, Left, Right,
                Forward2, Backward2, Left2, Right2, Reset, Fire, Boost, 
                Ability1, Ability2, Ability3, Ability4
            });
            
            

        }
        public bool ForwardDown() { return Forward.IsDown() || Forward2.IsDown(); }
        public bool BackwardDown() { return Backward.IsDown() || Backward2.IsDown(); }
        public bool LeftDown() { return Left.IsDown() || Left2.IsDown(); }
        public bool RightDown() { return Right.IsDown() || Right2.IsDown(); }

        public bool KeyDownOnce(Key key)
        {
            if(key.IsDown() && !GameState.keysDown.Contains(key))
            {
                GameState.keysDown.Add(key);
                return true;
            }
            return false;
        }

    }
    public class JsonKeys
    {
        public Keys KeyEnter { get; set; }
        public Keys KeyEscape { get; set; }
        public Keys KeyForward { get; set; }
        public Keys KeyBackward { get; set; }
        public Keys KeyLeft { get; set; }
        public Keys KeyRight { get; set; }

        public Keys KeyForward2 { get; set; }
        public Keys KeyBackward2 { get; set; }
        public Keys KeyLeft2 { get; set; }
        public Keys KeyRight2 { get; set; }

        public Keys KeyReset{ get; set; }

        public Keys KeyFire { get; set; }
        public Keys KeyBoost{ get; set; }
        public Keys Ability1 { get; set; }
        public Keys Ability2 { get; set; }
        public Keys Ability3 { get; set; }
        public Keys Ability4 { get; set; }

        public static JsonKeys LoadFromJson(string filePath)
        {
            // Read JSON file content
            string jsonContent = File.ReadAllText(filePath);

            // Deserialize JSON to KeyMappings object
            JsonKeys jsonKeys = JsonConvert.DeserializeObject<JsonKeys>(jsonContent);


            return jsonKeys;
        }
        public void SaveToJson(string filePath)
        {
            string jsonContent = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, jsonContent);
        }

        public void UpdateKey(string propertyName, Keys newKey)
        {
            // Use reflection to find the property by name
            var property = typeof(KeyMappings).GetProperty(propertyName);

            if (property != null && property.PropertyType == typeof(Keys))
            {
                // Set the new key value
                property.SetValue(this, newKey);
            }
            else
            {
                throw new ArgumentException("Invalid property name or type.");
            }
        }
        // Modify key mappings (e.g., user presses a key in the game)
        //keyMappings.UpdateKey("KeyAccelerate", Keys.Space);

        // Save the updated key mappings to JSON file
        //keyMappings.SaveToJson("path/to/your/keymappings.json");
    }
}
