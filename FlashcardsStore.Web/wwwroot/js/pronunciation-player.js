/**
 * Kholoud Elnady English Academy - Interactive Audio Pronunciation Player
 * Native Web Speech API with dual accents (US/UK) and speed toggle (Normal / Slow for Kids).
 */
document.addEventListener('alpine:init', () => {
    Alpine.data('pronunciationPlayer', (wordsOrElementId = []) => {
        let initialWords = [];
        if (typeof wordsOrElementId === 'string') {
            const el = document.getElementById(wordsOrElementId);
            if (el) {
                try {
                    initialWords = JSON.parse(el.textContent || '[]');
                } catch (e) {
                    console.error('Error parsing words json:', e);
                }
            }
        } else if (Array.isArray(wordsOrElementId)) {
            initialWords = wordsOrElementId;
        }

        return {
            words: initialWords,
            currentWord: null,
            currentType: null, // 'word' or 'sentence'
            isPlaying: false,
            isAllPlaying: false,
            currentIndex: -1,
            accent: 'US', // 'US' or 'UK'
            rate: 0.9,    // 0.9 normal, 0.72 for slow for kids
            pitch: 1.05,
            voices: [],
            speechSynth: typeof window !== 'undefined' && 'speechSynthesis' in window ? window.speechSynthesis : null,
            isSupported: typeof window !== 'undefined' && 'speechSynthesis' in window,

        init() {
            if (!this.isSupported) return;
            this.loadVoices();
            if (this.speechSynth.onvoiceschanged !== undefined) {
                this.speechSynth.onvoiceschanged = () => this.loadVoices();
            }
        },

        loadVoices() {
            if (!this.isSupported) return;
            this.voices = this.speechSynth.getVoices() || [];
        },

        getBestVoice(accent) {
            const langCode = accent === 'UK' ? 'en-GB' : 'en-US';
            const matchedVoices = this.voices.filter(v => v.lang.replace('_', '-').startsWith(langCode));
            
            // Prefer natural / premium educational voices if available
            const preferred = matchedVoices.find(v => 
                v.name.includes('Natural') || 
                v.name.includes('Online') || 
                v.name.includes('Samantha') || 
                v.name.includes('Jenny') || 
                v.name.includes('Google') ||
                v.name.includes('Daniel')
            );
            return preferred || matchedVoices[0] || this.voices.find(v => v.lang.startsWith('en')) || null;
        },

        speak(text, type = 'word', wordItem = null) {
            if (!this.isSupported || !text) return;

            this.speechSynth.cancel();

            const utterance = new SpeechSynthesisUtterance(text);
            const voice = this.getBestVoice(this.accent);
            if (voice) {
                utterance.voice = voice;
            }
            utterance.lang = this.accent === 'UK' ? 'en-GB' : 'en-US';
            utterance.rate = this.rate;
            utterance.pitch = this.pitch;

            this.currentWord = wordItem ? wordItem.word : text;
            this.currentType = type;
            this.isPlaying = true;

            utterance.onstart = () => {
                this.isPlaying = true;
            };

            utterance.onend = () => {
                this.isPlaying = false;
                this.currentWord = null;
                this.currentType = null;
            };

            utterance.onerror = (e) => {
                console.warn('SpeechSynthesis error:', e);
                this.isPlaying = false;
                this.currentWord = null;
                this.currentType = null;
            };

            this.speechSynth.speak(utterance);
        },

        playWord(item) {
            this.isAllPlaying = false;
            this.speak(item.word, 'word', item);
        },

        playSentence(item) {
            this.isAllPlaying = false;
            this.speak(item.exampleSentence || item.word, 'sentence', item);
        },

        async playAll() {
            if (!this.words || this.words.length === 0 || !this.isSupported) return;

            if (this.isAllPlaying) {
                this.stop();
                return;
            }

            this.isAllPlaying = true;
            for (let i = 0; i < this.words.length; i++) {
                if (!this.isAllPlaying) break;
                this.currentIndex = i;
                const item = this.words[i];
                await this.speakAsync(item.word, 'word', item);
                // Pause between words for kids to repeat after the speaker
                if (this.isAllPlaying && i < this.words.length - 1) {
                    await new Promise(r => setTimeout(r, 850));
                }
            }
            this.isAllPlaying = false;
            this.currentIndex = -1;
            this.currentWord = null;
            this.currentType = null;
        },

        speakAsync(text, type, item) {
            return new Promise((resolve) => {
                if (!this.isSupported || !text) {
                    resolve();
                    return;
                }

                this.speechSynth.cancel();
                const utterance = new SpeechSynthesisUtterance(text);
                const voice = this.getBestVoice(this.accent);
                if (voice) utterance.voice = voice;
                utterance.lang = this.accent === 'UK' ? 'en-GB' : 'en-US';
                utterance.rate = this.rate;
                utterance.pitch = this.pitch;

                this.currentWord = item.word;
                this.currentType = type;
                this.isPlaying = true;

                utterance.onend = () => {
                    this.isPlaying = false;
                    resolve();
                };

                utterance.onerror = () => {
                    this.isPlaying = false;
                    resolve();
                };

                this.speechSynth.speak(utterance);
            });
        },

        stop() {
            if (this.isSupported && this.speechSynth) {
                this.speechSynth.cancel();
            }
            this.isPlaying = false;
            this.isAllPlaying = false;
            this.currentIndex = -1;
            this.currentWord = null;
            this.currentType = null;
        },

        setAccent(newAccent) {
            this.accent = newAccent;
            if (this.isPlaying) {
                this.stop();
            }
        },

        toggleSpeed() {
            this.rate = this.rate === 0.9 ? 0.72 : 0.9;
        }
    };
});
});

// Global Quick Pronunciation function for any card or element
window.playQuickCardPronunciation = function(btnElement, textToSay) {
    if (!('speechSynthesis' in window) || !textToSay) return;

    window.speechSynthesis.cancel();

    const utterance = new SpeechSynthesisUtterance(textToSay);
    utterance.lang = 'en-US';
    utterance.rate = 0.82;
    utterance.pitch = 1.05;

    const labelSpan = btnElement.querySelector('.audio-btn-label');
    const waveSpan = btnElement.querySelector('.soundwave-container');
    const speakerIcon = btnElement.querySelector('.speaker-icon');

    utterance.onstart = () => {
        btnElement.classList.add('ring-2', 'ring-amber-400', 'bg-[#D4AF37]/40');
        if (labelSpan) labelSpan.classList.add('hidden');
        if (waveSpan) waveSpan.classList.remove('hidden');
        if (speakerIcon) speakerIcon.classList.add('animate-pulse', 'text-amber-300');
    };

    const cleanup = () => {
        btnElement.classList.remove('ring-2', 'ring-amber-400', 'bg-[#D4AF37]/40');
        if (labelSpan) labelSpan.classList.remove('hidden');
        if (waveSpan) waveSpan.classList.add('hidden');
        if (speakerIcon) speakerIcon.classList.remove('animate-pulse', 'text-amber-300');
    };

    utterance.onend = cleanup;
    utterance.onerror = cleanup;

    window.speechSynthesis.speak(utterance);
};

// Global Teacher Voice Note Player for uploaded audio recordings
let currentPlayingAudio = null;
window.playAudioNote = function(btnElement, audioUrl) {
    if (!audioUrl) return;

    if (currentPlayingAudio) {
        currentPlayingAudio.pause();
        currentPlayingAudio = null;
        document.querySelectorAll('.audio-quick-btn').forEach(b => {
            b.classList.remove('ring-2', 'ring-emerald-400', 'bg-emerald-600');
            const wave = b.querySelector('.soundwave-container');
            if (wave) wave.classList.add('hidden');
        });
    }

    const audio = new Audio(audioUrl);
    currentPlayingAudio = audio;

    const waveSpan = btnElement.querySelector('.soundwave-container');
    btnElement.classList.add('ring-2', 'ring-emerald-400', 'bg-emerald-600');
    if (waveSpan) waveSpan.classList.remove('hidden');

    audio.onended = () => {
        btnElement.classList.remove('ring-2', 'ring-emerald-400', 'bg-emerald-600');
        if (waveSpan) waveSpan.classList.add('hidden');
        currentPlayingAudio = null;
    };

    audio.onerror = () => {
        btnElement.classList.remove('ring-2', 'ring-emerald-400', 'bg-emerald-600');
        if (waveSpan) waveSpan.classList.add('hidden');
        currentPlayingAudio = null;
    };

    audio.play();
};
