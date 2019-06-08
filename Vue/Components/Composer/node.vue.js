module.exports = {
    name: "ComposerNode",
    
    data() {
        return {
            note: -1
        }
    },

    template: `
    <input v-model="note" type="number" class="small-input" v-on:input="updateNote($event)">
    `,

    methods: {
        updateNote($event) {
            this.$emit('update-note', Number($event.target.value));
        }
    }
}
