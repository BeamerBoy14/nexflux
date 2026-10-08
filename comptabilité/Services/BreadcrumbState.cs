namespace comptabilité.Services;

// Permet à une page de détail de donner un libellé lisible (ex. le nom de la voiture)
// au dernier élément du fil d'Ariane, à la place de « Détail ».
public class BreadcrumbState
{
    public string? Section { get; private set; }
    public string? DetailLabel { get; private set; }

    public event Action? Changed;

    public void SetDetailLabel(string? section, string? label)
    {
        if (string.Equals(Section, section, StringComparison.Ordinal)
            && string.Equals(DetailLabel, label, StringComparison.Ordinal))
        {
            return;
        }

        Section = section;
        DetailLabel = label;
        Changed?.Invoke();
    }
}
