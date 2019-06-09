module.exports = {
    name: "ComposerNode",

    props: {
        editable: {
            type: Boolean,
            default: true
        },

        note: {
            type: Number,
            required: true
        }
    },
    
    data() {
        return {
            note: -1
        }
    },

    template: `
    <input :readonly="!editable" v-model="note" type="number" class="small-input" v-on:input="updateNote($event)">
    `,

    methods: {
        updateNote($event) {
            this.$emit('update-note', Number($event.target.value));
        }
    }
}
