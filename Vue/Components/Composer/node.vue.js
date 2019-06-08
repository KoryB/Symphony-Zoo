module.exports = {
    name: "ComposerNode",
    
    data() {
        return {
            note: -1
        }
    },

    template: `
    <input v-model="note" type="number" class="small-input">
    `
}
