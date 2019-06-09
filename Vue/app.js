var ComposerPanel = require('./Components/Composer/composer-panel.vue.js')

const v = new Vue({
    el: '#app',
    data: {
        editable: true,
        mode: ''
    },

    components: {
        ComposerPanel,
    }
})