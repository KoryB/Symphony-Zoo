var ComposerNode = require('./node.vue.js')
var MidiWriter = require('midi-writer-js')
var MidiParser = require('midi-parser-js')

module.exports = {
    name: "Composer",

    props: {
        editable: {
            type: Boolean,
            default: true
        },

        upperNotes: {
            type: Array,
            default: [-1, -1, -1, -1, -1, -1, -1, -1]
        },

        lowerNotes: {
            type:Array,
            default: [-1, -1, -1, -1, -1, -1, -1, -1]
        },

        guid: {
            type: String,
            default: "-1"
        }
    },
    
    data() {
        return {

        }
    },

    components: {
        ComposerNode
    },

    template: `
    <div>
        <div class="composer">
            <div class="input-grid">
                <div><composer-node :editable="editable" v-for="i in Array(8).keys()" :key="i"   :note="upperNotes[i]" v-on:update-note="updateUpperNote(i, $event)"/></div>
                <div><composer-node :editable="editable" v-for="i in Array(8).keys()" :key="i+7" :note="lowerNotes[i]" v-on:update-note="updateLowerNote(i, $event)"/></div>
            </div>
            <button v-on:click="postMidi" v-if="editable">Send!</button>
            <button v-on:click="previewMidi">Play</button>
        </div>
    </div>
    `,

    computed: {
        editable() {
            return this.guid !== "-1"
        }
    },

    mounted() {
        //console.log(MidiWriter)
    },

    methods: {
        updateUpperNote(index, value) {
            this.upperNotes[index] = value;
        },

        updateLowerNote(index, value) {
            this.lowerNotes[index] = value;
        },

        postMidi() {
            var upperTrack = new MidiWriter.Track()
            var lowerTrack = new MidiWriter.Track()
            
            this.upperNotes.forEach(item => {
                var event;

                if (item < 0) {
                    event = new MidiWriter.NoteEvent({pitch: [0], duration: 8})
                } else {
                    event = new MidiWriter.NoteEvent({pitch: [item], duration: 8})
                }

                upperTrack.addEvent(event)
            });

            this.lowerNotes.forEach(item => {
                var event;

                if (item < 0) {
                    event = new MidiWriter.NoteEvent({pitch: [item], duration: 8, wait: 8})
                } else {
                    event = new MidiWriter.NoteEvent({pitch: [item], duration: 8})
                }

                lowerTrack.addEvent(event)
            });

            var writer = new MidiWriter.Writer([upperTrack, lowerTrack])

            var item =
            {
                guid: this.guid,
                midiData: writer.base64()
            }

            $.ajax({
                type: "POST",
                accepts: "application/json",
                url: "/api/compose",
                contentType: "application/json",
                data: JSON.stringify(item),
                error: function(jqXHR, textStatus, errorThrown) {
                    console.log(jqXHR, textStatus, errorThrown)
                },
                success: function(result) {
                    console.log(result);
                }
            });
        },

        previewMidi() {
            console.log("Nice tunes dude!");
            console.log("ok");

            var filter = new Tone.Filter({
                type: 'bandpass',
                Q: 12
            }).toMaster()

            //schedule a series of frequency changes
            filter.frequency.setValueAtTime('C5', 0)
            filter.frequency.setValueAtTime('E5', 0.5)
            filter.frequency.setValueAtTime('G5', 1)
            filter.frequency.setValueAtTime('B5', 1.5)
            filter.frequency.setValueAtTime('C6', 2)
            filter.frequency.linearRampToValueAtTime('C1', 3)

            var noise = new Tone.Noise("brown").connect(filter).start(0).stop(3)

            //schedule an amplitude curve
            noise.volume.setValueAtTime(-20, 0)
            noise.volume.linearRampToValueAtTime(20, 2)
            noise.volume.linearRampToValueAtTime(-Infinity, 3)
            console.log("ok, bruv. you right. you right.");
        }
    }
}
