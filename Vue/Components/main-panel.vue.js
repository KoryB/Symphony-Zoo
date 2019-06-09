var ComposerPanel = require('./Composer/composer-panel.vue.js')
var MidiParser = require('midi-parser-js')

module.exports = {
    name: "MainPanel",
    
    data() {
        return {
            recentGetResponse: []
        }
    },

    components: {
        ComposerPanel
    },

    methods: {
        parseTrack(track) {
            const eventList = track.event;
            const noteOn = 9;
            const noteOff = 8;

            var notes = [];

            for (event of eventList) {
                if (event.type === noteOn) {
                    const pitch = event.data[0];

                    notes.push((pitch === 0)? -1 : pitch)
                }
            }

            return notes
        },

        getNewMeasure() {
            const me = this;

            $.ajax({
                type: "GET",
                url: "/api/compose",
                cache: false,

                success: function(data) {
                    me.recentGetResponse = data;
                    console.log("GET Response", me.recentGetResponse);
                }
            });
        }
    },

    computed: {
        composerInfo() {
            const newInfo = this.recentGetResponse.map(measure => {
                const guid = measure.guid;
                const midiDataBase64 = measure.midiData;

                console.log("Updating composer", guid, midiDataBase64);

                var info = {
                    upperNotes: [-1, -1, -1, -1, -1, -1, -1, -1],
                    lowerNotes: [-1, -1, -1, -1, -1, -1, -1, -1],
                    guid: "-1"
                }

                if (! /^[\-0]+$/g.test(guid)) {
                    info.guid = guid;
                }

                if (midiDataBase64 !== null) {
                    const midi = MidiParser.parse(midiDataBase64)

                    info.upperNotes = this.parseTrack(midi.track[0])
                    info.lowerNotes = this.parseTrack(midi.track[1])
                }

                return info
            })

            console.log("New info", newInfo)

            return newInfo
        }
    },
    
    template: `
    <div>
        <button @click="getNewMeasure">Try a new measure</button>
        <composer-panel :composer-info="composerInfo"></composer-panel>
    </div>
    `
}