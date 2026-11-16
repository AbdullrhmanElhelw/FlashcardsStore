/**
 * Kholoud Elnady English Academy - Admin In-Browser Voice Note Recorder
 * Allows Miss Kholoud to record real voice notes directly from her microphone
 * or upload audio files (mp3, wav, m4a, ogg, webm) for flashcard packs.
 */
document.addEventListener('alpine:init', () => {
    Alpine.data('voiceNoteRecorder', (existingAudioUrl = '') => ({
        isRecording: false,
        hasRecording: false,
        recordingTime: 0,
        timerInterval: null,
        audioPreviewUrl: existingAudioUrl || null,
        hasExistingAudio: !!existingAudioUrl,
        existingAudioUrl: existingAudioUrl || '',
        mediaRecorder: null,
        audioChunks: [],
        statusMessage: '',
        errorMessage: '',
        isPlayingPreview: false,
        previewAudioEl: null,

        init() {
            if (this.hasExistingAudio) {
                this.statusMessage = 'يوجد تسجيل صوتي مسجل مسبقاً للباقة';
            }
        },

        async startRecording() {
            this.errorMessage = '';
            this.statusMessage = '';

            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                this.errorMessage = 'متصفحك لا يدعم تسجيل الصوت المباشر. يمكنك رفع ملف صوتي جاهز من جهازك بدلاً من ذلك.';
                return;
            }

            try {
                const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
                this.audioChunks = [];

                // Prefer webm or ogg
                let options = {};
                if (MediaRecorder.isTypeSupported('audio/webm;codecs=opus')) {
                    options = { mimeType: 'audio/webm;codecs=opus' };
                } else if (MediaRecorder.isTypeSupported('audio/ogg;codecs=opus')) {
                    options = { mimeType: 'audio/ogg;codecs=opus' };
                }

                this.mediaRecorder = new MediaRecorder(stream, options);

                this.mediaRecorder.ondataavailable = (e) => {
                    if (e.data && e.data.size > 0) {
                        this.audioChunks.push(e.data);
                    }
                };

                this.mediaRecorder.onstop = () => {
                    const mimeType = this.mediaRecorder.mimeType || 'audio/webm';
                    const audioBlob = new Blob(this.audioChunks, { type: mimeType });
                    this.audioPreviewUrl = URL.createObjectURL(audioBlob);
                    this.hasRecording = true;
                    this.statusMessage = `تم تسجيل فويس نوت بنجاح (${this.formatTime(this.recordingTime)}) ✓`;

                    // Attach to file input
                    const fileInput = document.getElementById('audioFileInput');
                    if (fileInput) {
                        const fileExt = mimeType.includes('ogg') ? 'ogg' : 'webm';
                        const audioFile = new File([audioBlob], `voice-note-${Date.now()}.${fileExt}`, { type: mimeType });
                        const dataTransfer = new DataTransfer();
                        dataTransfer.items.add(audioFile);
                        fileInput.files = dataTransfer.files;
                    }

                    // Stop all microphone audio tracks
                    stream.getTracks().forEach(track => track.stop());
                };

                this.mediaRecorder.start(200);
                this.isRecording = true;
                this.recordingTime = 0;
                this.timerInterval = setInterval(() => {
                    this.recordingTime++;
                }, 1000);

            } catch (err) {
                console.error('Microphone access error:', err);
                this.errorMessage = 'تعذر الوصول للميكروفون. يرجى التأكد من إعطاء إذن الميكروفون للمتصفح.';
            }
        },

        stopRecording() {
            if (this.mediaRecorder && this.isRecording) {
                clearInterval(this.timerInterval);
                this.isRecording = false;
                this.mediaRecorder.stop();
            }
        },

        resetRecording() {
            if (this.isRecording) {
                this.stopRecording();
            }
            this.hasRecording = false;
            this.audioPreviewUrl = this.hasExistingAudio ? this.existingAudioUrl : null;
            this.recordingTime = 0;
            this.statusMessage = '';
            this.errorMessage = '';

            const fileInput = document.getElementById('audioFileInput');
            if (fileInput) {
                fileInput.value = '';
            }
        },

        handleManualFileUpload(event) {
            const file = event.target.files[0];
            if (file) {
                this.audioPreviewUrl = URL.createObjectURL(file);
                this.hasRecording = true;
                this.statusMessage = `تم تحديد الملف الصوتي: ${file.name} ✓`;
                this.errorMessage = '';
            }
        },

        formatTime(seconds) {
            const m = Math.floor(seconds / 60);
            const s = seconds % 60;
            return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        }
    }));
});
