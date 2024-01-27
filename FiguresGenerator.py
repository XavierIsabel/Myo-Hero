import matplotlib.pyplot as plt
import numpy as np

def plot_graph( x_values, y_values, x_label, y_label, title):
    # Plot the graph on the given s
    plt.figure()
    plt.plot(x_values, y_values, marker='o', linestyle='-', label=title)

    plt.xlabel(x_label)
    plt.ylabel(y_label)
    plt.title(title)
    plt.grid(True)
    plt.legend()

def scatter_graph( x_values, y_values, x_label, y_label, title):
    # Plot the graph on the given s
    plt.figure()
    plt.plot(x_values, y_values, marker='o', linestyle='', label=title)

    plt.xlabel(x_label)
    plt.ylabel(y_label)
    plt.title(title)
    plt.grid(True)
    plt.legend()

def main():
    # Replace this file path with the actual path to your .txt file
    file_path = './Subject_6/Stats_BioPoint_EMG_2024-01-27T10-25-13Z.txt'

    # Open the specified .txt file
    with open(file_path, 'r') as file:
        lines = file.readlines()

    # Extract data from lines
    time = list(map(int, lines[1].strip().split(":")[1].split()))
    for i in range(len(time)):
        if i == 0:
            pass
        else:
            time[i] = time[i] + time[i-1]
    notes = list(map(int, lines[0].strip().split(":")[1].split()))
    notes_lengths = list(map(int, lines[1].strip().split(":")[1].split()))
    hold_accuracy = list(map(float, lines[2].strip().split(":")[1].split()))
    in_timing_accuracy = list(map(float, lines[3].strip().split(":")[1].split()))
    out_timing_accuracy = list(map(float, lines[4].strip().split(":")[1].split()))
    classifications = list(map(float, lines[5].strip().split(":")[1].split()))
    timestamps = list(map(float, lines[6].strip().split(":")[1].split()))

    # Plot graphs on separate subplots
    scatter_graph(timestamps, classifications, 'time', 'classifications', 'Classifications over time')
    plot_graph(time, hold_accuracy, 'time', 'Hold Accuracy', 'Hold Accuracy For Notes')
    plot_graph(time, out_timing_accuracy, 'time', 'OUT-Timing Accuracy', 'OUT-Timing Accuracy For Notes')
    plot_graph(time, in_timing_accuracy, 'time', 'IN-Timing Accuracy', 'IN-Timing Accuracy For Notes')

    scatter_graph(notes_lengths, hold_accuracy, 'Notes Lengths', 'Hold Accuracy', 'Hold Accuracy For Notes in function of Notes_lengths')
    scatter_graph(notes, hold_accuracy, 'Notes', 'Hold Accuracy', 'Hold Accuracy For Notes in function of Notes')
    scatter_graph(notes_lengths, in_timing_accuracy, 'Notes Lengths', 'IN-Timing Accuracy', 'IN-Timing Accuracy For Notes in function of Notes_lengths')
    scatter_graph(notes, in_timing_accuracy, 'Notes', 'IN-Timing Accuracy', 'IN-Timing Accuracy For Notes in function of Notes')
    scatter_graph(notes_lengths, out_timing_accuracy, 'Notes Lengths', 'OUT-Timing Accuracy', 'OUT-Timing Accuracy For Notes in function of Notes_lengths')
    scatter_graph(notes, out_timing_accuracy, 'Notes', 'OUT-Timing Accuracy', 'OUT-Timing Accuracy For Notes in function of Notes')

    # Adjust layout for better spacing
    plt.tight_layout()

    # Show all plots
    plt.show()

if __name__ == "__main__":
    main()