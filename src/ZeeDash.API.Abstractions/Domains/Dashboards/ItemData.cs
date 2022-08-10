namespace ZeeDash.API.Abstractions.Domains.Dashboards;

public class ItemData
    : Dictionary<string, object?> {

    public ItemData() {
    }

    public ItemData(params string[] defaultKeys) : this(new Dictionary<string, object?>(), defaultKeys) {
    }

    public ItemData(Dictionary<string, object?> values, params string[] defaultKeys) : base(values) {
        foreach (var key in defaultKeys) {
            if (values.ContainsKey(key)) {
                this.Set(key, values[key]);
            } else {
                this.SetDefaultToNullIfNotContained(key);
            }
        }
    }

    protected object? Get(string key) {
        if (this.ContainsKey(key)) {
            return this[key];
        }

        return null;
    }

    protected void Set(string key, object? value) {
        if (this.ContainsKey(key)) {
            this[key] = value;
        } else {
            this.Add(key, value);
        }
    }

    protected void SetDefaultToNullIfNotContained(string key) {
        if (!this.ContainsKey(key)) {
            this.Add(key, null);
        }
    }
}
