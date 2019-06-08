var ComposerNode = require('./node.vue.js')
var MidiWriter = require('midi-writer-js')

module.exports = {
    name: "Composer",
    
    data() {
        return {
            upperNotes: [-1, -1, -1, -1, -1, -1, -1, -1],
            lowerNotes: [-1, -1, -1, -1, -1, -1, -1, -1]
        }
    },

    components: {
        ComposerNode
    },

    template: `
    <div>
        <div class="composer">
            <div><composer-node v-for="i in Array(8).keys()" :key="i"   v-on:update-note="updateUpperNote(i, $event)"/></div>
            <div><composer-node v-for="i in Array(8).keys()" :key="i+7" v-on:update-note="updateLowerNote(i, $event)"/></div>
            <button v-on:click="postMidi">Send your masterpiece!</button>
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
        }
    }
}
