module.exports = {
    name: "TestComponent",
    
    data() {
        return {
            message: "Hellooooo!!!!"
        }
    },

    template: `
    <div>
        <p> {{ message }} </p>
    </div>
    `
}
