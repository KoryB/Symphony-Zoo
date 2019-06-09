var Composer = require('./Components/Composer/composer.vue.js')

const v = new Vue({
    el: '#app',
    data: {
        editable: true,
        mode: ''
    },

    components: {
        Composer,
    }
})