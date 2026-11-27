namespace FlashcardsStore.PhonicsGame.Models;

public sealed record GrammarChallenge(
    string Topic,
    string TitleAr,
    string TitleEn,
    string PromptTemplate,
    string CorrectOption,
    IReadOnlyList<string> Options,
    string HintAr,
    string HintEn);

public sealed record GrammarPack(IReadOnlyList<GrammarChallenge> Questions);
