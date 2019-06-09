var Composer = require('./composer.vue.js')

module.exports = {
    name: "ComposerPanel",

    props: {
        composerInfo: {
            type:Array,

            required:true
        }
    },
    
    data() {
        return {
            style: {
                display: "grid",
                gridTemplateColumns: "auto auto"
            }
        }
    },

    mounted() {
        console.log("composer info", this.composerInfo)
    },

    components: {
        Composer
    },

    template: `
    <div :style="style">
        <composer v-for="(info, index) in composerInfo" 
            :key="index" 
            :editable="index == 1"
            :upper-notes="info.upperNotes"
            :lower-notes="info.lowerNotes"
            :guid="info.guid">
        </composer>
    </div>
    `
}