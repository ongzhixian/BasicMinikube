import os
from datetime import datetime

from flask import Flask, jsonify, send_from_directory


app = Flask(__name__)


@app.route('/favicon.ico')
def favicon():
    return send_from_directory(
        os.path.join(app.root_path, 'static'),
        'favicon.ico', 
        mimetype='image/vnd.microsoft.icon')

@app.get("/")
def hello():
    return jsonify({
        "message": "Hello from Sample-Flask-App1",
        "timestamp": datetime.now()
    }), 200


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5050, debug=True)
