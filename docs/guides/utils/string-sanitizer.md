# String Sanitization

The `StringSanitizer` utility provides a standardized way to transform human-readable input into machine-friendly, URL-safe strings. This is primarily used for generating "slugs" (e.g., converting `"My Awesome Story"` into `"my_awesome_story"`) which are used as identifiers in URLs and internal routing.

---

## 1. The `Normalize` Method

The core functionality of the utility is contained within the `Normalize` method. It follows a strict transformation pipeline to ensure consistent output.

### Transformation Pipeline

1.  **Case Normalization**: Converts the entire input string to lowercase invariant.
2.  **Regex Cleaning**: Uses a regular expression (`[^a-z0-9]+`) to identify any character that is not a lowercase letter or a digit.
3.  **Separator Replacement**: Replaces all identified special characters and whitespace with a single underscore (`_`).
4.  **Trimming**: Removes any leading or trailing underscores from the final result.

---

## 2. Usage Examples

The following table demonstrates how different inputs are processed by the `Normalize` method:

| Input String | Output (Slug) | Notes |
| :--- | :--- | :--- |
| `"Dark Tone"` | `dark_tone` | Space replaced by underscore |
| `"The Hero's Journey!"` | `the_hero_s_journey` | Special characters removed/replaced |
| `"  Space...  "` | `space` | Leading/trailing underscores trimmed |
| `"123-ABC_xyz"` | `123_abc_xyz` | Mixed alphanumeric and separators |

### Code Implementation Example

```csharp
// Basic usage in a controller or service
string userInput = "A New Adventure!";
string slug = StringSanitizer.Normalize(userInput);

// Result: "a_new_adventure"
```

---

## 3. Technical Constraints

* **Character Set**: The output is strictly limited to lowercase `a-z`, digits `0-9`, and the underscore `_`.
* **Empty Input**: If the input is null, empty, or consists only of whitespace, the method returns an empty string (`string.Empty`).
* **Lossy Transformation**: This process is "lossy"—once a string is normalized, you cannot perfectly reconstruct the original casing or special characters from the slug alone.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Consistency**: Ensures all URL slugs follow the same format across the entire system. | **Information Loss**: Cannot preserve case sensitivity, accented characters, or punctuation. |
| **Safety**: Produces strings that are safe for URLs and file systems, avoiding many injection/parsing issues. | **Single-Pass Logic**: The regex approach is efficient but simple; it does not handle complex linguistic rules (e.transliteration). |

***
*Last Updated: [Date]*
