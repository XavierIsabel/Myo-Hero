import random

def generate_gesture_file(total_time, filename, neutral_duration=2):
    current_time = 0
    data = ""

    while current_time < total_time:
        # Choose a random gesture (1, 2, or 3) and a random duration (1 to 4 seconds)
        gesture = random.choice([0, 1, 2, 3])
        duration = random.randint(1, 4)

        # Make sure the total time is not exceeded
        if current_time + duration > total_time:
            duration = total_time - current_time

        # Append the gesture and its duration to the data string
        data += f"{current_time}:{gesture}:{duration}\n"

        # Update the current time
        current_time += duration
        current_time -= 1

        # Insert a "0" gesture, if there's still time remaining
        if current_time + neutral_duration <= total_time:
            #data += f"{current_time}:-1:{neutral_duration}\n"
            current_time += neutral_duration
            current_time -= 1
        else:
            break  # Break the loop if adding a "0" gesture would exceed total time

    # Write the data to a file
    with open(filename, 'w') as file:
        file.write(data[:-1])

    print(f"Gesture file '{filename}' has been generated.")


#Usage example
generate_gesture_file(total_time=180, filename='Assets/Resources/song0.txt', neutral_duration = 3)