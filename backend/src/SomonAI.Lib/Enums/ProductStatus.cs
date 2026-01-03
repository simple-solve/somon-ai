namespace SomonAI.Lib.Enums;

/// <summary>
/// Product listing status
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Product is being created/edited
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Product is published and visible
    /// </summary>
    Published = 1,

    /// <summary>
    /// Product is sold/inactive
    /// </summary>
    Archived = 2
}