using System.Text.RegularExpressions;

namespace FlashcardsStore.Application.Common;

/// <summary>
/// Provides curated and dynamically extracted vocabulary words, phonetic notation, and example sentences
/// for interactive native audio pronunciation preview in the store and 3D card viewers.
/// </summary>
public static class PronunciationWordExtractor
{
    private static readonly Dictionary<string, List<PronunciationItem>> CuratedPackWords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["short-vowels-pack"] = new()
        {
            new() { Word = "Cat", Phonetic = "/kæt/ • Short a", IconEmoji = "🐱", ExampleSentence = "The cat sat on the soft mat." },
            new() { Word = "Bat", Phonetic = "/bæt/ • Short a", IconEmoji = "🦇", ExampleSentence = "A bat flies high at night." },
            new() { Word = "Hat", Phonetic = "/hæt/ • Short a", IconEmoji = "🎩", ExampleSentence = "He wore a shiny red hat." },
            new() { Word = "Bed", Phonetic = "/bɛd/ • Short e", IconEmoji = "🛏️", ExampleSentence = "It is time to sleep in bed." },
            new() { Word = "Sun", Phonetic = "/sʌn/ • Short u", IconEmoji = "☀️", ExampleSentence = "The warm sun shines bright." }
        },
        ["blends-and-digraphs-pack"] = new()
        {
            new() { Word = "Ship", Phonetic = "/ʃɪp/ • 'sh' digraph", IconEmoji = "🚢", ExampleSentence = "A big ship sails across the sea." },
            new() { Word = "Fish", Phonetic = "/fɪʃ/ • 'sh' digraph", IconEmoji = "🐟", ExampleSentence = "The colorful fish swims fast." },
            new() { Word = "Shell", Phonetic = "/ʃɛl/ • 'sh' digraph", IconEmoji = "🐚", ExampleSentence = "I found a shell on the beach." },
            new() { Word = "Star", Phonetic = "/stɑːr/ • 'st' blend", IconEmoji = "⭐", ExampleSentence = "Look at that glowing star." },
            new() { Word = "Clock", Phonetic = "/klɒk/ • 'cl' blend", IconEmoji = "⏰", ExampleSentence = "The clock ticks in the room." }
        },
        ["cvc-word-builder"] = new()
        {
            new() { Word = "Bed", Phonetic = "/b-e-d/", IconEmoji = "🛏️", ExampleSentence = "Rest your head on the bed." },
            new() { Word = "Dog", Phonetic = "/d-o-g/", IconEmoji = "🐶", ExampleSentence = "A playful dog runs around." },
            new() { Word = "Pen", Phonetic = "/p-e-n/", IconEmoji = "🖊️", ExampleSentence = "Write neatly with a pen." },
            new() { Word = "Sun", Phonetic = "/s-u-n/", IconEmoji = "☀️", ExampleSentence = "Warm and bright yellow sun." },
            new() { Word = "Cup", Phonetic = "/k-ʌ-p/", IconEmoji = "☕", ExampleSentence = "A warm cup of hot milk." }
        },
        ["parts-of-speech-pack"] = new()
        {
            new() { Word = "Puppy", Phonetic = "Noun (اسم)", IconEmoji = "🐶", ExampleSentence = "The happy puppy plays outside." },
            new() { Word = "Happy", Phonetic = "Adjective (صفة)", IconEmoji = "😊", ExampleSentence = "She feels happy and excited." },
            new() { Word = "Run", Phonetic = "Verb (فعل)", IconEmoji = "🏃", ExampleSentence = "Children run fast in the yard." },
            new() { Word = "Quickly", Phonetic = "Adverb (حال)", IconEmoji = "⚡", ExampleSentence = "The rabbit hops quickly." },
            new() { Word = "In", Phonetic = "Preposition (حرف جر)", IconEmoji = "📥", ExampleSentence = "The book is in the bag." }
        },
        ["verb-tenses-pack"] = new()
        {
            new() { Word = "Go", Phonetic = "Present Tense", IconEmoji = "🚶", ExampleSentence = "I go to school every morning." },
            new() { Word = "Went", Phonetic = "Past Simple", IconEmoji = "🔙", ExampleSentence = "She went to the library yesterday." },
            new() { Word = "Gone", Phonetic = "Past Participle", IconEmoji = "✨", ExampleSentence = "They have gone to the club." },
            new() { Word = "Eat", Phonetic = "Present Tense", IconEmoji = "🍎", ExampleSentence = "We eat fresh apples daily." },
            new() { Word = "Ate", Phonetic = "Past Simple", IconEmoji = "🍽️", ExampleSentence = "He ate a delicious lunch." }
        },
        ["sentence-building-cards"] = new()
        {
            new() { Word = "The cat sleeps", Phonetic = "Complete Sentence", IconEmoji = "🐱", ExampleSentence = "The cat sleeps peacefully on the rug." },
            new() { Word = "Where is the book?", Phonetic = "Question Form", IconEmoji = "❓", ExampleSentence = "Where is your English storybook?" },
            new() { Word = "I love reading", Phonetic = "Action Sentence", IconEmoji = "📖", ExampleSentence = "I love reading fun stories every day." },
            new() { Word = "Look at the sky!", Phonetic = "Exclamation", IconEmoji = "🌌", ExampleSentence = "Look at the stars in the dark sky!" }
        },
        ["500-words-kids-pack"] = new()
        {
            new() { Word = "Cow", Phonetic = "/kaʊ/", IconEmoji = "🐮", ExampleSentence = "The cow gives delicious fresh milk." },
            new() { Word = "Horse", Phonetic = "/hɔːrs/", IconEmoji = "🐴", ExampleSentence = "The horse gallops fast across the field." },
            new() { Word = "Sheep", Phonetic = "/ʃiːp/", IconEmoji = "🐑", ExampleSentence = "A gentle sheep with soft wool." },
            new() { Word = "Duck", Phonetic = "/dʌk/", IconEmoji = "🦆", ExampleSentence = "The duck paddles in the blue pond." },
            new() { Word = "Rooster", Phonetic = "/ˈruːstər/", IconEmoji = "🐓", ExampleSentence = "The rooster crows early at dawn." }
        },
        ["school-vocabulary-pack"] = new()
        {
            new() { Word = "Ruler", Phonetic = "/ˈruːlər/", IconEmoji = "📏", ExampleSentence = "May I borrow your wooden ruler?" },
            new() { Word = "Pencil", Phonetic = "/ˈpɛnsəl/", IconEmoji = "✏️", ExampleSentence = "Write your name with a sharp pencil." },
            new() { Word = "Book", Phonetic = "/bʊk/", IconEmoji = "📚", ExampleSentence = "Open your English book to page twelve." },
            new() { Word = "Teacher", Phonetic = "/ˈtiːtʃər/", IconEmoji = "👩‍🏫", ExampleSentence = "Listen carefully to the teacher." },
            new() { Word = "Desk", Phonetic = "/dɛsk/", IconEmoji = "🪑", ExampleSentence = "Keep your school desk clean and tidy." }
        },
        ["dolch-sight-words-box"] = new()
        {
            new() { Word = "the", Phonetic = "Sight Word", IconEmoji = "🔤", ExampleSentence = "The bright sun is high in the sky." },
            new() { Word = "said", Phonetic = "Sight Word", IconEmoji = "💬", ExampleSentence = "Mom said it is time for breakfast." },
            new() { Word = "where", Phonetic = "Sight Word", IconEmoji = "🗺️", ExampleSentence = "Where did you put your pencil?" },
            new() { Word = "come", Phonetic = "Sight Word", IconEmoji = "👋", ExampleSentence = "Come and play a fun game with us." },
            new() { Word = "play", Phonetic = "Sight Word", IconEmoji = "⚽", ExampleSentence = "We play outside in the sunshine." }
        },
        ["fry-100-words-pack"] = new()
        {
            new() { Word = "there", Phonetic = "Sight Word", IconEmoji = "👉", ExampleSentence = "Look at the colorful flowers over there." },
            new() { Word = "their", Phonetic = "Sight Word", IconEmoji = "👥", ExampleSentence = "This is their new English classroom." },
            new() { Word = "they're", Phonetic = "Sight Word", IconEmoji = "🌟", ExampleSentence = "They're learning new English words quickly." },
            new() { Word = "people", Phonetic = "Sight Word", IconEmoji = "🧑‍🤝‍🧑", ExampleSentence = "Many friendly people came to visit." },
            new() { Word = "write", Phonetic = "Sight Word", IconEmoji = "✍️", ExampleSentence = "Write an interesting story in your notebook." }
        }
    };

    public static IReadOnlyList<PronunciationItem> GetWordsForProduct(
        string? slug,
        string? name,
        string? backPreviewText,
        string? categoryName = null,
        string? customSampleWords = null)
    {
        // 1. Admin-configured custom sample words take highest priority
        if (!string.IsNullOrWhiteSpace(customSampleWords))
        {
            var customList = ParseCustomSampleWords(customSampleWords);
            if (customList.Count > 0)
            {
                return customList;
            }
        }

        // 2. Curated pack lookup by slug
        if (!string.IsNullOrWhiteSpace(slug) && CuratedPackWords.TryGetValue(slug.Trim(), out var curated) && curated.Count > 0)
        {
            return curated;
        }

        var result = new List<PronunciationItem>();

        if (!string.IsNullOrWhiteSpace(backPreviewText))
        {
            // 1. Check for single-quoted words: 'word'
            var singleQuotes = Regex.Matches(backPreviewText, @"'([a-zA-Z\s]{2,20})'");
            foreach (Match m in singleQuotes)
            {
                var val = m.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(val) && result.All(x => !x.Word.Equals(val, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(new PronunciationItem
                    {
                        Word = val,
                        Phonetic = "Flashcard Word",
                        IconEmoji = "🔤",
                        ExampleSentence = $"Practice saying: '{val}'"
                    });
                }
            }

            // 2. Check for words after 'as in ...'
            if (result.Count == 0 && backPreviewText.Contains("as in", StringComparison.OrdinalIgnoreCase))
            {
                var afterAsIn = backPreviewText.Substring(backPreviewText.IndexOf("as in", StringComparison.OrdinalIgnoreCase) + 5);
                var dotIndex = afterAsIn.IndexOfAny(new[] { '.', ';', '-' });
                if (dotIndex > 0) afterAsIn = afterAsIn.Substring(0, dotIndex);

                var parts = afterAsIn.Split(new[] { ',', '&', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    var clean = Regex.Replace(p, @"[^a-zA-Z]", "").Trim();
                    if (clean.Length >= 2 && result.All(x => !x.Word.Equals(clean, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.Add(new PronunciationItem
                        {
                            Word = clean,
                            Phonetic = "Sound Pattern",
                            IconEmoji = "🎯",
                            ExampleSentence = $"Listen and repeat: {clean}"
                        });
                    }
                }
            }
        }

        return result.Take(12).ToList();
    }

    private static List<PronunciationItem> ParseCustomSampleWords(string raw)
    {
        var items = new List<PronunciationItem>();
        var entries = raw.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var entry in entries)
        {
            var parts = entry.Split('|', 2, StringSplitOptions.TrimEntries);
            var word = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(word)) continue;
            if (items.Any(x => x.Word.Equals(word, StringComparison.OrdinalIgnoreCase))) continue;

            var sentence = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
                ? parts[1].Trim()
                : $"Practice saying: '{word}'";

            items.Add(new PronunciationItem
            {
                Word = word,
                Phonetic = "Card Word",
                IconEmoji = "🔊",
                ExampleSentence = sentence
            });

            if (items.Count >= 12) break;
        }

        return items;
    }
}
