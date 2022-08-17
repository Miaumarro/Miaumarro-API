namespace MiauDatabase.Enums;

/// <summary>
/// Represents the tags a product can have.
/// </summary>
[Flags]
public enum ProductTag
{
    /// <summary>
    /// The product contains no tags.
    /// </summary>
    None = 0,

    /// <summary>
    /// The product is meant for small sized pets.
    /// </summary>
    SmallPet = 1 << 0,

    /// <summary>
    /// The product is meant for medium sized pets.
    /// </summary>
    MediumPet = 1 << 1,

    /// <summary>
    /// The product is meant for large sized pets.
    /// </summary>
    LargePet = 1 << 2,

    /// <summary>
    /// The product is meant to be dressed up.
    /// </summary>
    Clothing = 1 << 3,

    /// <summary>
    /// The product is meant to treat illnesses.
    /// </summary>
    Medicine = 1 << 4,

    /// <summary>
    /// The product is a furniture meant to be used by the pet (cat tower, dog bed, etc).
    /// </summary>
    Furniture = 1 << 5,

    /// <summary>
    /// The product is edible.
    /// </summary>
    Food = 1 << 6,

    /// <summary>
    /// The product is a toy.
    /// </summary>
    Toy = 1 << 7,

    /// <summary>
    /// The product is an accessory (clothing pieces, leashes, etc).
    /// </summary>
    Accessory = 1 << 8,

    /// <summary>
    /// The product is meant for dogs.
    /// </summary>
    Dog = 1 << 9,

    /// <summary>
    /// The product is meant for cats.
    /// </summary>
    Cat = 1 << 10,

    /// <summary>
    /// The product is meant for birds.
    /// </summary>
    Bird = 1 << 11,

    /// <summary>
    /// The product is meant for fishes.
    /// </summary>
    Fish = 1 << 12,

    /// <summary>
    /// The product is meant for other kinds of pets.
    /// </summary>
    OtherPet = 1 << 13
}