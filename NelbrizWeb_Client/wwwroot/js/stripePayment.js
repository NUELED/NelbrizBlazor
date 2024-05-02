redirectToCheckout = function (sessionId) {
    var stripe = Stripe("pk_test_51P5H1TEy8LsW1YLGvtXMCMRGlO4qXNUOUg1vV4HXiK8deuhoXuWD5ixbfGopa8uVbXqiHfTFLrtRAHZXmCm9Tpyn00IqakTq5M")
    stripe.redirectToCheckout({ sessionId: sessionId });
}