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
        style() {
            
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
            console.log("Starting fetch")
            
            var xhttp = new XMLHttpRequest();
            var me = this;

            xhttp.onreadystatechange = function() {
                if (this.readyState == 4 && this.status == 200) {
                    const responseJSON = JSON.parse(xhttp.responseText);
                    const guid = responseJSON[1].guid;

                    var upperTrack = new MidiWriter.Track()
                    var lowerTrack = new MidiWriter.Track()
                    
                    me.upperNotes.forEach(item => {
                        var event;
        
                        if (item < 0) {
                            event = new MidiWriter.NoteEvent({pitch: [item], duration: 8, wait: 8})
                        } else {
                            event = new MidiWriter.NoteEvent({pitch: [item], duration: 8})
                        }
        
                        upperTrack.addEvent(event)
                    });
        
                    me.lowerNotes.forEach(item => {
                        var event;
        
                        if (item < 0) {
                            event = new MidiWriter.NoteEvent({pitch: [item], duration: 8, wait: 8})
                        } else {
                            event = new MidiWriter.NoteEvent({pitch: [item], duration: 8})
                        }
        
                        lowerTrack.addEvent(event)
                    });
        
                    var writer = new MidiWriter.Writer([upperTrack, lowerTrack])
        
                    console.log(writer.dataUri())
                    
                    var xhttpPOST = new XMLHttpRequest();
                    xhttpPOST.onreadystatechange = function() {
                        if (this.readyState == 4 && this.status == 200) {
                            console.log("Sent it off!")
                        }

                        console.log(this.status);
                    }
                    xhttpPOST.open("POST", "http://localhost:5000/api/graph", true)
                    xhttpPOST.setRequestHeader("Content-type", "application/json");
                    xhttpPOST.send({
                        guid: guid,
                        midiData: writer.dataUri()
                    });
                }
            };
            xhttp.open("GET", "http://localhost:5000/api/Graph", true);
            xhttp.send();
        },

        previewMidi() {
            console.log("Nice tunes dude!");
        }
    }
}
