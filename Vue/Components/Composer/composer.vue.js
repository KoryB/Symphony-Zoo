var ComposerNode = require('./node.vue.js')
var MidiWriter = require('midi-writer-js')

module.exports = {
    name: "Composer",
    
    data() {
        return {
            upperNotes: [0, 1, 2, 3, 4, 5, 6, 7],
            lowerNotes: [10, 11, 12, 13, 14, 15, 16, 17],
            notes: [-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1]
        }
    },

    components: {
        ComposerNode
    },

    template: `
    <div>
        <p>{{notes}}</p>
        <form class="composer">
            <div><composer-node v-for="i in upperNotes" :key="i" v-on:update-note="updateNote(i, $event)"/></div>
            <div><composer-node v-for="i in lowerNotes" :key="i" /></div>
        </form>
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
        updateNote(index, $event) {
            this.notes[index] = Number($event.target.value);
            console.log(this.notes)
        }
    }


}
