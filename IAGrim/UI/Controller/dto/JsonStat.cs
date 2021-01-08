namespace IAGrim.UI.Controller.dto {
    public class JsonStat {

        public string Text;
        public float? Param0;
        public float? Param1;
        public float? Param2;

        public string Param3;
        public float? Param4;
        public string Param5;
        public string Param6;

        public string Extras { get; set; }

        public override string ToString() {
            return Text + Param0 + Param1 + Param2 + Param3 + Param4 + Param5 + Param6 + Extras;
        }

        public int CompareTo(JsonStat obj) {
            return ToString().CompareTo(obj.ToString());
        }

        public override bool Equals(object obj) {
            JsonStat o = obj as JsonStat;
            return this.ToString().Equals(obj?.ToString());
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}
