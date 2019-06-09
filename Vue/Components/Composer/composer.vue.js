var ComposerNode = require('./node.vue.js')
var MidiWriter = require('midi-writer-js')

module.exports = {
    name: "Composer",
    
    data() {
        return {
            upperNotes: [0, 1, 2, 3, 4, 5, 6, 7],
            lowerNotes: [10, 11, 12, 13, 14, 15, 16, 17]
        }
    },

    components: {
        ComposerNode
    },

    template: `
    <form class="composer">
        <div><composer-node v-for="i in upperNotes" :key="i" /></div>
        <div><composer-node v-for="i in lowerNotes" :key="i" /></div>
    </form>
    `,

    computed: {
        style() {
            
        }
    },

    mounted() {
        console.log(MidiWriter)
    }


}
